using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ODBP.Authentication;
using ODBP.Data;

namespace ODBP.Features.Beheer
{
    [ApiController]
    [Route("api/beheer/homepage")]
    [Authorize(AdminPolicy.Name)]
    public class HomepageController(OdbpDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken token)
        {
            var resources = await context.Resources.SingleAsync(token);

            return Ok(new { welcome = resources?.Welcome });
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] HomepageRequest request, CancellationToken token)
        {
            var resources = await context.Resources.SingleAsync(token);

            resources.Welcome = request.Welcome;

            await context.SaveChangesAsync(token);

            return Ok(new { welcome = request.Welcome });
        }
    }

    public record HomepageRequest(string? Welcome);
}
