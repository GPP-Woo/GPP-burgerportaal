using Microsoft.AspNetCore.Mvc;

namespace ODBP.Features.Afbeeldingen
{
    public enum ImageType
    {
        Logo,
        Favicon,
        Image
    }

    [ApiController]
    [Route("api/afbeeldingen")]
    public class AfbeeldingenController() : ControllerBase
    {
        private static readonly Dictionary<ImageType, string> s_defaultFileNames = new()
        {
            { ImageType.Logo, "default-logo.svg" },
            { ImageType.Favicon, "default-favicon.ico" },
            { ImageType.Image, "default-image.jpg" }
        };

        private static readonly Dictionary<string, string> s_mimeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            { ".svg", "image/svg+xml" },
            { ".png", "image/png" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".gif", "image/gif" },
            { ".ico", "image/x-icon" },
            { ".webp", "image/webp" }
        };

        [HttpGet("{fileName}")]
        public IActionResult Get(string fileName)
        {
            var type = GetImageTypeFromFilename(fileName);

            if (type == null)
            {
                return NotFound();
            }

            // If it's just the type name, serve default
            if (fileName.Equals(type.Value.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return ServeDefaultImage(type.Value);
            }
            
            var filePath = Path.Combine(StorageConfig.ImagesPath, fileName);

            // If image does not exists, serve default
            if (!System.IO.File.Exists(filePath))
            {
                return ServeDefaultImage(type.Value);
            }

            return ServeFile(filePath);
        }

        private static ImageType? GetImageTypeFromFilename(string filename)
        {
            if (filename.StartsWith("logo", StringComparison.OrdinalIgnoreCase))
                return ImageType.Logo;
            if (filename.StartsWith("favicon", StringComparison.OrdinalIgnoreCase))
                return ImageType.Favicon;
            if (filename.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                return ImageType.Image;

            return null;
        }

        private IActionResult ServeDefaultImage(ImageType type)
        {
            var defaultFileName = s_defaultFileNames[type];

            var defaultPath = Path.Combine(AppContext.BaseDirectory, "Resources", defaultFileName);

            return !System.IO.File.Exists(defaultPath) ? NotFound() : ServeFile(defaultPath);
        }

        private FileStreamResult ServeFile(string filePath)
        {
            var extension = Path.GetExtension(filePath);

            var contentType = s_mimeTypes.TryGetValue(extension, out var mime) ? mime : "application/octet-stream";

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return File(fileStream, contentType);
        }
    }
}
