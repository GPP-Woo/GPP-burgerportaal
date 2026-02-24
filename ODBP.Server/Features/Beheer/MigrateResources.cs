using Microsoft.EntityFrameworkCore;
using ODBP.Data;
using ODBP.Features.Afbeeldingen;

namespace ODBP.Features.Beheer;

/// <summary>
/// Handles the migration and initialization of resource URLs and associated images in the database, ensuring that all
/// required resources are available and stored correctly.
/// </summary>
/// <remarks>This class ensures that resource URLs are populated and that images such as the favicon, logo, and
/// main image are downloaded and stored in the configured directory if they are not already present. It checks for
/// existing resources and updates them as necessary, supporting initial setup and ongoing migrations.</remarks>
/// <param name="context">The database context used to access and update resource entities.</param>
/// <param name="configuration">The configuration provider that supplies URLs and settings for the required resources.</param>
/// <param name="storageConfig">The storage configuration that specifies paths and settings for saving images and other resource files.</param>
/// <param name="clientFactory">The HTTP client factory used to create clients for downloading images from specified URLs.</param>
public class MigrateResources(OdbpDbContext context, IConfiguration configuration, StorageConfig storageConfig, IHttpClientFactory clientFactory, ILogger<MigrateResources> logger)
{
    public async Task ExecuteAsync()
    {
        try
        {
            var resources = await context.Resources.SingleAsync();

            resources.A11yUrl ??= configuration["RESOURCES:TOEGANKELIJKHEIDSVERKLARING_REGISTER_URL"];
            resources.ContactUrl ??= configuration["RESOURCES:GEMEENTE_CONTACT_URL"];
            resources.PrivacyUrl ??= configuration["RESOURCES:GEMEENTE_PRIVACY_URL"];
            resources.VideoUrl ??= configuration["RESOURCES:GEMEENTE_VIDEO_URL"];
            resources.WebsiteUrl ??= configuration["RESOURCES:GEMEENTE_WEBSITE_URL"];
            resources.Welcome ??= configuration["RESOURCES:GEMEENTE_WELKOM"];

            using var client = clientFactory.CreateClient();
            resources.FaviconFileName ??= await HandleImage(configuration["RESOURCES:GEMEENTE_FAVICON_URL"], ImageType.Favicon, client);
            resources.ImageFileName ??= await HandleImage(configuration["RESOURCES:GEMEENTE_MAIN_IMAGE_URL"], ImageType.Image, client);
            resources.LogoFileName ??= await HandleImage(configuration["RESOURCES:GEMEENTE_LOGO_URL"], ImageType.Logo, client);

            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogWarning(e, """
                An error occurred while migrating resources settings from obsolete environment variables to the database.
                Please check the configuration and ensure that all URLs are correct and accessible or remove the environment variables entirely:
                `RESOURCES:TOEGANKELIJKHEIDSVERKLARING_REGISTER_URL`
                `RESOURCES:GEMEENTE_CONTACT_URL`
                `RESOURCES:GEMEENTE_PRIVACY_URL`
                `RESOURCES:GEMEENTE_VIDEO_URL`
                `RESOURCES:GEMEENTE_WEBSITE_URL`
                `RESOURCES:GEMEENTE_WELKOM`
                `RESOURCES:GEMEENTE_FAVICON_URL`
                `RESOURCES:GEMEENTE_MAIN_IMAGE_URL`
                `RESOURCES:GEMEENTE_LOGO_URL`
                """);
        }
    }

    private async Task<string?> HandleImage(string? url, ImageType type, HttpClient client)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;

        var extension = Path.GetExtension(url).ToLowerInvariant();

        // Ensure storage directory exists
        storageConfig.EnsureDirectoryExists();

        // Generate unique filename
        var uniqueFileName = $"{type.ToString().ToLowerInvariant()}_{Guid.NewGuid()}{extension}";

        var filePath = Path.Combine(storageConfig.ImagesPath, uniqueFileName);

        // Save file
        await DownloadFile(client, url, filePath);

        return uniqueFileName;
    }

    private static async Task DownloadFile(HttpClient httpClient, string url, string destinationPath)
    {
        var response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = new FileStream(destinationPath, FileMode.Create);
            await stream.CopyToAsync(fileStream);
        }
    }
}
