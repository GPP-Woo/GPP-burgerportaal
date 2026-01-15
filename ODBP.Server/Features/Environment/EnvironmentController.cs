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

            var welcome = string.IsNullOrWhiteSpace(resources?.Welcome) 
                ? $"<p>Welkom op het Woo-burgerportaal van {resourcesConfig.Name}!</p>"
                : resources.Welcome;

            var response = new
            {
                resourcesConfig.Title,
                resourcesConfig.Name,
                resourcesConfig.LogoUrl,
                resourcesConfig.FaviconUrl,
                resourcesConfig.ImageUrl,
                resourcesConfig.TokensUrl,
                resourcesConfig.Theme,
                resourcesConfig.MediaUrl,
                resourcesConfig.VideoUrl,
                resourcesConfig.WebsiteUrl,
                resourcesConfig.PrivacyUrl,
                resourcesConfig.ContactUrl,
                Welcome = welcome,
                resourcesConfig.A11yUrl
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
