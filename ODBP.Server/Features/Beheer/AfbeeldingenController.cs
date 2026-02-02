using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ODBP.Authentication;
using ODBP.Data;
using ODBP.Features.Afbeeldingen;

namespace ODBP.Features.Beheer
{
    [ApiController]
    [Route("api/beheer/afbeeldingen")]
    [Authorize(AdminPolicy.Name)]
    public class AfbeeldingenController(OdbpDbContext context) : ControllerBase
    {
        private static readonly Dictionary<ImageType, string[]> s_allowedExtensions = new()
        {
            { ImageType.Logo, [".svg", ".png", ".jpg", ".jpeg", ".gif", ".webp"] },
            { ImageType.Favicon, [".ico", ".svg", ".png"] },
            { ImageType.Image, [".svg", ".png", ".jpg", ".jpeg", ".gif", ".webp"] }
        };

        private static readonly Dictionary<ImageType, long> s_maxSizes = new()
        {
            { ImageType.Logo, 2 * 1024 * 1024 }, // 2 MB
            { ImageType.Favicon, 512 * 1024 }, // 512 KB
            { ImageType.Image, 5 * 1024 * 1024 } // 5 MB
        };

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken token)
        {
            var resources = await context.Resources.SingleAsync(token);

            return Ok(new AfbeeldingenInfo(
                Logo: resources.LogoFileName,
                Favicon: resources.FaviconFileName,
                Image: resources.ImageFileName
            ));
        }

        [HttpPost("{type}")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Upload(ImageType type, IFormFile file, CancellationToken token)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Geen bestand geüpload." });
            }

            // Validate file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            var allowedExtensions = s_allowedExtensions[type];

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { message = $"Ongeldig bestandsformaat." });
            }

            // Validate file size
            var maxSize = s_maxSizes[type];

            if (file.Length > maxSize)
            {
                return BadRequest(new { message = $"Bestand is te groot." });
            }

            // Ensure storage directory exists
            StorageConfig.EnsureDirectoryExists();

            // Generate unique filename
            var uniqueFileName = $"{type.ToString().ToLowerInvariant()}_{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(StorageConfig.ImagesPath, uniqueFileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, token);
            }

            // Update database
            var resources = await context.Resources.SingleAsync(token);

            var oldFileName = type switch
            {
                ImageType.Logo => resources.LogoFileName,
                ImageType.Favicon => resources.FaviconFileName,
                ImageType.Image => resources.ImageFileName,
                _ => null
            };

            // Delete old file if exists
            if (!string.IsNullOrWhiteSpace(oldFileName))
            {
                var oldFilePath = Path.Combine(StorageConfig.ImagesPath, oldFileName);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Update database with new filename
            switch (type)
            {
                case ImageType.Logo:
                    resources.LogoFileName = uniqueFileName;
                    break;
                case ImageType.Favicon:
                    resources.FaviconFileName = uniqueFileName;
                    break;
                case ImageType.Image:
                    resources.ImageFileName = uniqueFileName;
                    break;
            }

            await context.SaveChangesAsync(token);

            return Ok(new { fileName = uniqueFileName });
        }
    }

    public record AfbeeldingenInfo(string? Logo, string? Favicon, string? Image);
}
