using System.ComponentModel.DataAnnotations;
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
                resources.WebsiteUrl ?? "",
                resources.PrivacyUrl ?? "",
                resources.ContactUrl ?? "",
                resources.A11yUrl ?? ""
            ));
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Links request, CancellationToken token)
        {
            var resources = await context.Resources.SingleAsync(token);

            resources.WebsiteUrl = NormalizeUrl(request.WebsiteUrl);
            resources.PrivacyUrl = NormalizeUrl(request.PrivacyUrl);
            resources.ContactUrl = NormalizeUrl(request.ContactUrl);
            resources.A11yUrl = NormalizeUrl(request.A11yUrl);

            await context.SaveChangesAsync(token);

            return Ok(new Links(
                resources.WebsiteUrl,
                resources.PrivacyUrl,
                resources.ContactUrl,
                resources.A11yUrl
            ));
        }

        private static string NormalizeUrl(string? url) =>
            Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri.ToString() : "";
    }

    public record Links(
        [HttpsUrl] string WebsiteUrl,
        [HttpsUrl] string PrivacyUrl,
        [HttpsUrl] string ContactUrl,
        [HttpsUrl] string A11yUrl
    );

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class HttpsUrlAttribute : DataTypeAttribute
    {
        public HttpsUrlAttribute() : base(DataType.Url) { }
        public override bool IsValid(object? value) => value switch
        {
            null or "" => true,
            string url => Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps,
            _ => false
        };
    }
}
