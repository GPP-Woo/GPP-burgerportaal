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

            resources.WebsiteUrl = new Uri(request.WebsiteUrl, UriKind.Absolute).ToString();
            resources.PrivacyUrl = new Uri(request.PrivacyUrl, UriKind.Absolute).ToString();
            resources.ContactUrl = new Uri(request.ContactUrl, UriKind.Absolute).ToString();
            resources.A11yUrl = new Uri(request.A11yUrl, UriKind.Absolute).ToString();

            await context.SaveChangesAsync(token);

            return Ok(new Links(
                resources.WebsiteUrl,
                resources.PrivacyUrl,
                resources.ContactUrl,
                resources.A11yUrl
            ));
        }
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
