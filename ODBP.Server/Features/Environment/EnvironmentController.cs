using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ODBP.Authentication;

namespace ODBP.Features.Environment
{
    [Route("api/environment")]
    [ApiController]
    public class EnvironmentController : ControllerBase
    {
        private record VersionInfo(string? SemanticVersion, string? GitSha);

        private readonly ResourcesConfig _resourcesConfig;
        private static readonly VersionInfo s_versionInfo = GetVersionInfo();

        public EnvironmentController(ResourcesConfig resourcesConfig)
        {
            _resourcesConfig = resourcesConfig;
        }

        [HttpGet("admin")]
        [Authorize(Policy = AdminPolicy.Name)]
        public IActionResult IsAdmin()
        {
            return Ok(new { message = "Gebruiker is admin." });
        }

        [HttpGet("resources")]
        public IActionResult GetResources()
        {
            var response = new
            {
                _resourcesConfig.Title,
                _resourcesConfig.Name,
                _resourcesConfig.LogoUrl,
                _resourcesConfig.FaviconUrl,
                _resourcesConfig.ImageUrl,
                _resourcesConfig.TokensUrl,
                _resourcesConfig.Theme,
                _resourcesConfig.MediaUrl,
                _resourcesConfig.VideoUrl,
                _resourcesConfig.WebsiteUrl,
                _resourcesConfig.PrivacyUrl,
                _resourcesConfig.ContactUrl,
                _resourcesConfig.Welcome,
                _resourcesConfig.A11yUrl
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
