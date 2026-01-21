using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ODBP.Authentication;
using ODBP.Config;
using ODBP.Data;

namespace ODBP.Features.Beheer
{
    [ApiController]
    [Route("api/beheer/homepage")]
    [Authorize(AdminPolicy.Name)]
    public partial class HomepageController(OdbpDbContext context, IMemoryCache cache) : ControllerBase
    {
        private const string InvalidError = "De waarde moet een geldig YouTube of Vimeo embed URL zijn.";

        [GeneratedRegex(@"^https://(www\.)?(youtube\.com|youtube-nocookie\.com)/embed/([\w-]+)")]
        private static partial Regex YouTubeEmbedRegex();

        [GeneratedRegex(@"^https://player\.vimeo\.com/video/(\d+)")]
        private static partial Regex VimeoEmbedRegex();

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken token)
        {
            var resources = await context.Resources.SingleAsync(token);

            return Ok(new { resources.Welcome, resources.VideoUrl });
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Homepage request, CancellationToken token)
        {
            var resources = await context.Resources.SingleAsync(token);

            var oldVideoUrl = resources.VideoUrl;

            if (!string.IsNullOrWhiteSpace(request.VideoUrl) && request.VideoUrl != oldVideoUrl)
            {
                var youtubeMatch = YouTubeEmbedRegex().Match(request.VideoUrl);
                var vimeoMatch = VimeoEmbedRegex().Match(request.VideoUrl);

                if (!(youtubeMatch.Success || vimeoMatch.Success))
                {
                    return BadRequest(new { message = InvalidError });
                }
            }

            resources.VideoUrl = request.VideoUrl;
            resources.Welcome = request.Welcome;

            await context.SaveChangesAsync(token);

            // Invalidate COEP cache if VideoUrl changed
            if (oldVideoUrl != resources.VideoUrl)
            {
                cache.InvalidateCoepCache();
            }

            return Ok(new { resources.Welcome, resources.VideoUrl });
        }
    }

    public record Homepage(string? Welcome, string? VideoUrl);
}
