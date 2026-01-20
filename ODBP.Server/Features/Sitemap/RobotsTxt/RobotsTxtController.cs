using Microsoft.AspNetCore.Mvc;

namespace ODBP.Features.Sitemap.RobotsTxt
{
    [ApiController]
    public class RobotsTxtController(BaseUri baseUri, IConfiguration config) : ControllerBase
    {
        [HttpGet("/robots.txt")]
        public IActionResult Get()
        {
            var blockRobots = bool.TryParse(config["BLOCK_ROBOTS"], out var b) && b;

            if (blockRobots)
            {
                return Ok("""
                User-agent: *
                Disallow: /
                """);
            }

            var sitemapIndexUri = new Uri(baseUri, ApiRoutes.SitemapIndex);

            return Ok($"""
            User-agent: *
            Disallow:
            Sitemap: {sitemapIndexUri}
            """);
        }
    }
}
