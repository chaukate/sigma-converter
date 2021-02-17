using Microsoft.AspNetCore.Http;
using Sigma.Converter.Enumerations;
using System;
using System.Drawing;
using System.IO;

namespace Sigma.Converter
{
    /// <summary>
    /// Imge Resize
    /// </summary>
    public static class ImageResize
    {
        private static double _height;
        private static double _width;
        private static Image _image;

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
            stream.Flush();
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
            stream.Flush();
            SetHeightWidth(_image);
            var resizedImage = _image.GetThumbnailImage((int)_width, (int)_height, () => false, IntPtr.Zero);
            var str = resizedImage.ToBase64String();
            return str;
        }

        #region Private Methods
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
        #endregion
    }
}
