using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Sigma.Converter.Enumerations;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Sigma.Converter
{
    /// <summary>
    /// Image Converter
    /// </summary>
    public static class ImageConverter
    {
        private static readonly string[] ContentTypes = { "image/apng", "image/avif", "image/bmp", "image/gif", "image/jfif", "image/jpeg", "image/png", "image/svg+xml", "image/tiff", "image/webp" };
        private static double _height;
        private static double _width;
        private static int _quality = 75;
        private static Image _image;

        /// <summary>
        /// Validates whether file content type is image type.
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public static bool IsValidImage(this IFormFile formFile)
        {
            if (formFile.Length > 0)
            {
                foreach (var contentType in ContentTypes)
                {
                    if (formFile.ContentType.ToLower() == contentType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Converts file to image
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public static Image ToImage(this IFormFile formFile)
        {
            if (formFile.IsValidImage())
            {
                using var stream = new MemoryStream();
                formFile.CopyTo(stream);
                var image = Image.FromStream(stream);
                return image;
            }
            throw new Exception("Invalid image file.");
        }

        /// <summary>
        /// Converts base64 string into FormFile
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="name"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static IFormFile ToFormFile(this string base64, string name = "filename", string extension = ".png")
        {
            var bytes = Convert.FromBase64String(base64);
            var stream = new MemoryStream(bytes, 0, bytes.Length);
            var formFile = new FormFile(stream, 0, stream.Length, null, $"{name}{extension}")
            {
                Headers = new HeaderDictionary(),
                ContentType = $"image/{extension.TrimStart('.')}"
            };
            stream.Flush();
            if (formFile.IsValidImage())
            {
                return formFile;
            }
            throw new Exception("Invalid file extension.");
        }

        /// <summary>
        /// Converts FormFile into base64 string
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public static string ToBase64String(this IFormFile formFile)
        {
            if (formFile.IsValidImage())
            {
                string base64 = "";
                using (var stream = new MemoryStream())
                {
                    formFile.CopyTo(stream);
                    var fileBytes = stream.ToArray();
                    base64 = Convert.ToBase64String(fileBytes);
                }
                return base64;
            }
            throw new Exception("Invalid image file.");
        }

        /// <summary>
        /// Converts image into base64 string
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string ToBase64String(this Image image)
        {
            string base64 = "";
            var stream = new MemoryStream();

            var qualityParamId = Encoder.Quality;
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(qualityParamId, _quality);
            var codec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
            image.Save(stream, codec, encoderParameters);

            var imageBytes = stream.ToArray();
            base64 = Convert.ToBase64String(imageBytes);
            return base64;
        }

        /// <summary>
        /// Sets size
        /// </summary>
        /// <param name="size"></param>
        private static void SetSize(ImageSizes size)
        {
            switch (size)
            {
                case ImageSizes.x32:
                    _height = 32;
                    _width = 32;
                    break;
                case ImageSizes.x64:
                    _height = 64;
                    _width = 64;
                    break;
                case ImageSizes.x128:
                    _height = 128;
                    _width = 128;
                    break;
                case ImageSizes.x150:
                    _height = 150;
                    _width = 150;
                    break;
                case ImageSizes.x256:
                    _height = 256;
                    _width = 256;
                    break;
                case ImageSizes.x800:
                    _height = 800;
                    _width = 800;
                    break;
                default:
                    throw new Exception("Invalid size.");
            }
        }

        /// <summary>
        /// Resize image to defined size type and return image. x32 is height and weight = 32.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Image ResizeToImage(this Image image, ImageSizes size)
        {
            SetSize(size);
            _image = image;
            SetHeightWidth(_image);
            var resizedImage = _image.GetThumbnailImage((int)_width, (int)_height, () => false, IntPtr.Zero);
            return resizedImage;
        }

        /// <summary>
        /// Resize image to defined size type and return as base64 string. x32 is height and weight = 32.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string ResizeToBase64(this Image image, ImageSizes size)
        {
            SetSize(size);
            _image = image;
            SetHeightWidth(_image);
            var resizedImage = _image.GetThumbnailImage((int)_width, (int)_height, () => false, IntPtr.Zero);
            var base64 = resizedImage.ToBase64String();
            return base64;
        }

        /// <summary>
        /// Resize to defined size type and return image. x32 is height and weight = 32.
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Image ResizeToImage(this IFormFile formFile, ImageSizes size)
        {
            SetSize(size);
            _image = formFile.ToImage();
            SetHeightWidth(_image);
            var resizedImage = _image.GetThumbnailImage((int)_width, (int)_height, () => false, IntPtr.Zero);
            return resizedImage;
        }

        /// <summary>
        /// Resize to defined size type and return base64 string. x32 is height and weight = 32.
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string ResizeToBase64(this IFormFile formFile, ImageSizes size)
        {
            SetSize(size);
            _image = formFile.ToImage();
            SetHeightWidth(_image);
            var resizedImage = _image.GetThumbnailImage((int)_width, (int)_height, () => false, IntPtr.Zero);
            var base64 = resizedImage.ToBase64String();
            return base64;
        }

        /// <summary>
        /// Resize to defined size type and return image. x32 is height and weight = 32.
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Image ResizeToImage(this string base64, ImageSizes size)
        {
            var bytes = Convert.FromBase64String(base64);
            var stream = new MemoryStream(bytes, 0, bytes.Length);
            SetSize(size);
            _image = Image.FromStream(stream);
            SetHeightWidth(_image);
            var resizedImage = _image.GetThumbnailImage((int)_width, (int)_height, () => false, IntPtr.Zero);
            return resizedImage;
        }

        /// <summary>
        /// Resize to defined size type and return base64 string. x32 is height and weight = 32.
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string ResizeToBase64(this string base64, ImageSizes size)
        {
            var bytes = Convert.FromBase64String(base64);
            var stream = new MemoryStream(bytes, 0, bytes.Length);
            SetSize(size);
            _image = Image.FromStream(stream);
            SetHeightWidth(_image);
            var resizedImage = _image.GetThumbnailImage((int)_width, (int)_height, () => false, IntPtr.Zero);
            var str = resizedImage.ToBase64String();
            return str;
        }

        /// <summary>
        /// Sets height and width.
        /// </summary>
        /// <param name="image"></param>
        private static void SetHeightWidth(Image image)
        {
            double ratio;
            if (image.Height <= _height && image.Width <= _width)
            {
                _height = image.Height;
                _width = image.Width;
            }
            else if (image.Height > image.Width)
            {
                if (image.Height <= _height)
                {
                    _height = image.Height;
                    _width = _height;
                }
                ratio = (double)image.Width / (double)image.Height;
                _width *= ratio;
            }
            else if (image.Width >= image.Height)
            {
                if (image.Width <= _width)
                {
                    _width = image.Width;
                    _height = _width;
                }
                ratio = (double)image.Height / (double)image.Width;
                _height *= ratio;
            }
        }
    }
}
