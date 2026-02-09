using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ODBP.Authentication;
using ODBP.Data;

namespace ODBP.Features.Beheer
{
    [ApiController]
    [Route("api/beheer/links")]
    [Authorize(AdminPolicy.Name)]
    public class LinksBeheerController(OdbpDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken token)
        {
            var resources = await context.Resources.SingleAsync(token);

            return Ok(new Links(
                resources.WebsiteUrl,
                resources.PrivacyUrl,
                resources.ContactUrl,
                resources.A11yUrl
            ));
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Links request, CancellationToken token)
        {
            if (InvalidUrl(request.WebsiteUrl, "websiteUrl", out var websiteUrl)
                is BadRequestObjectResult errorWebsiteUrl) return errorWebsiteUrl;
            if (InvalidUrl(request.PrivacyUrl, "privacyUrl", out var privacyUrl)
                is BadRequestObjectResult errorPrivacyUrl) return errorPrivacyUrl;
            if (InvalidUrl(request.ContactUrl, "contactUrl", out var contactUrl)
                is BadRequestObjectResult errorContactUrl) return errorContactUrl;
            if (InvalidUrl(request.A11yUrl, "a11yUrl", out var a11yUrl)
                is BadRequestObjectResult errorA11yUrl) return errorA11yUrl;

            var resources = await context.Resources.SingleAsync(token);

            resources.WebsiteUrl = websiteUrl;
            resources.PrivacyUrl = privacyUrl;
            resources.ContactUrl = contactUrl;
            resources.A11yUrl = a11yUrl;

            await context.SaveChangesAsync(token);

            return Ok(new Links(
                resources.WebsiteUrl,
                resources.PrivacyUrl,
                resources.ContactUrl,
                resources.A11yUrl
            ));
        }

        private BadRequestObjectResult? InvalidUrl(string? url, string fieldName, out string? normalized)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                normalized = null;

                return null;
            }

            if (Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps)
            {
                normalized = uri.ToString();

                return null;
            }

            normalized = null;

            return BadRequest(new { message = $"De waarde moet een geldig URL zijn: {fieldName}" });
        }
    }

    public record Links(string? WebsiteUrl, string? PrivacyUrl, string? ContactUrl, string? A11yUrl);
}
