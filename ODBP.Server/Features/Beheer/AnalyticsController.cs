using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Memory;
using ODBP.Authentication;
using ODBP.Config;
using ODBP.Data;
using ODBP.Data.Entities;

namespace ODBP.Features.Beheer
{
    [ApiController]
    [Route("api/beheer/analytics")]
    [Authorize(AdminPolicy.Name)]
    public class AnalyticsBeheerController(OdbpDbContext context, IMemoryCache cache) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken token)
        {
            var resources = await context.Resources.SingleAsync(token);

            return Ok(ToAnalytics(resources));
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Analytics request, CancellationToken token)
        {
            var resources = await context.Resources.SingleAsync(token);

            resources.AnalyticsTool = request.Tool;

            // bij "geen" worden de koppelgegevens gewist
            resources.AnalyticsUrl = request.Tool == AnalyticsTool.None ? null : NormalizeUrl(request.Url);
            resources.AnalyticsIdentifier = request.Tool == AnalyticsTool.None ? null : request.Identifier?.Trim();

            await context.SaveChangesAsync(token);
            cache.InvalidateAnalyticsCache();

            return Ok(ToAnalytics(resources));
        }

        private static Analytics ToAnalytics(Resources resources) => new(
            resources.AnalyticsTool,
            resources.AnalyticsUrl ?? "",
            resources.AnalyticsIdentifier ?? ""
        );

        private static string? NormalizeUrl(string? url) =>
            Uri.TryCreate(url?.Trim(), UriKind.Absolute, out var uri) ? uri.ToString() : null;
    }

    public partial record Analytics(AnalyticsTool Tool, string? Url, string? Identifier) : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Enum.IsDefined(Tool))
            {
                yield return new ValidationResult("De gekozen analytics-tool is onbekend.", [nameof(Tool)]);
                yield break;
            }

            if (Tool == AnalyticsTool.Matomo)
            {
                if (!Uri.TryCreate(Url?.Trim(), UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
                {
                    yield return new ValidationResult(
                        "De opgegeven waarde is geen valide URL. Let op dat de URL moet beginnen met https://.",
                        [nameof(Url)]);
                }

                if (!NumericIdentifier().IsMatch(Identifier?.Trim() ?? ""))
                {
                    yield return new ValidationResult("Het site-ID is verplicht en moet een getal zijn.", [nameof(Identifier)]);
                }
            }
        }

        [GeneratedRegex(@"^\d{1,9}$")]
        private static partial Regex NumericIdentifier();
    }
}
