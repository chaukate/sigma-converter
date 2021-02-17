using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
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
        private static int _quality = 99;

        /// <summary>
        /// Validates whether file content type is image type.
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public static bool IsValidImage(this IFormFile formFile)
        {
            if (formFile.Length > 0)
            {
                return ContentTypes.Contains(formFile.ContentType.ToLower());
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
                var stream = new MemoryStream();
                formFile.CopyTo(stream);
                var image = Image.FromStream(stream);
                stream.Flush();
                return image;
            }
            throw new Exception("Invalid image file.");
        }

        /// <summary>
        /// Converts base64 string to image.
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static Image ToImage(this string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            var stream = new MemoryStream(bytes, 0, bytes.Length);
            var image = Image.FromStream(stream);
            stream.Flush();
            return image;
        }

        /// <summary>
        /// Converts base64 string into FormFile
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="name"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static IFormFile ToFormFile(this string base64, string name = "filename", string extension = ".jpeg")
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
                var stream = new MemoryStream();
                formFile.CopyTo(stream);
                var fileBytes = stream.ToArray();
                string base64 = Convert.ToBase64String(fileBytes);
                stream.Flush();
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
            var stream = new MemoryStream();

            var qualityParamId = Encoder.Quality;
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(qualityParamId, _quality);
            var codec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
            image.Save(stream, codec, encoderParameters);

            var imageBytes = stream.ToArray();
            var base64 = Convert.ToBase64String(imageBytes);
            stream.Flush();
            return base64;
        }
    }
}
