using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ODBP.Data;

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
    public class AfbeeldingenController(OdbpDbContext context, StorageConfig storageConfig) : ControllerBase
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

        [HttpGet("{type}")]
        public async Task<IActionResult> Get(ImageType type, CancellationToken token)
        {
            var resources = await context.Resources.SingleAsync(token);

            var fileName = type switch
            {
                ImageType.Logo => resources.LogoFileName,
                ImageType.Favicon => resources.FaviconFileName,
                ImageType.Image => resources.ImageFileName,
                _ => null
            };

            // If no custom image is set, try and serve default
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return ServeDefaultImage(type);
            }

            // If image does not exists, try and serve default
            var filePath = Path.Combine(storageConfig.ImagesPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return ServeDefaultImage(type);
            }

            return ServeFile(filePath);
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
