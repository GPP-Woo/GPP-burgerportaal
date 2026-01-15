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
    public class HomepageController(OdbpDbContext context, IMemoryCache cache) : ControllerBase
    {
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
