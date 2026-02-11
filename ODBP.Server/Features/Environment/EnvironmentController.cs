using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ODBP.Data;

namespace ODBP.Features.Environment
{
    [Route("api/environment")]
    [ApiController]
    public class EnvironmentController(ResourcesConfig resourcesConfig, OdbpDbContext context) : ControllerBase
    {
        private record VersionInfo(string? SemanticVersion, string? GitSha);

        private static readonly VersionInfo s_versionInfo = GetVersionInfo();

        [HttpGet("resources")]
        public async Task<IActionResult> GetResources(CancellationToken token)
        {
            var resources = await context.Resources.SingleAsync(token);

            var welcomeText = string.IsNullOrWhiteSpace(resources?.Welcome) 
                ? $"<p>Welkom op het Woo-burgerportaal van {resourcesConfig.OrganisationLabel} {resourcesConfig.OrganisationName}!</p>"
                : resources.Welcome;

            var logoUrl = $"/api/afbeeldingen/{resources?.LogoFileName ?? "logo"}";
            var faviconUrl = $"/api/afbeeldingen/{resources?.FaviconFileName ?? "favicon"}";
            var imageUrl = $"/api/afbeeldingen/{resources?.ImageFileName ?? "image"}";

            var response = new
            {
                resourcesConfig.PortalTitle,
                resourcesConfig.OrganisationName,
                resourcesConfig.OrganisationLabel,
                logoUrl,
                faviconUrl,
                imageUrl,
                resourcesConfig.TokensUrl,
                resourcesConfig.ThemeName,
                resources?.VideoUrl,
                resources?.WebsiteUrl,
                resources?.PrivacyUrl,
                resources?.ContactUrl,
                welcomeText,
                resources?.A11yUrl
            };

            return Ok(response);
        }

        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            return Ok(s_versionInfo);
        }

        private static VersionInfo GetVersionInfo()
        {
            var parts = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion
                ?.Split('+') ?? [];
            return new VersionInfo(parts.ElementAtOrDefault(0), parts.ElementAtOrDefault(1));
        }
    }
}
