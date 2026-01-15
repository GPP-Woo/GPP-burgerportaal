using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ODBP.Data;

namespace ODBP.Config
{
    /// <summary>
    /// Middleware that dynamically adds Cross-Origin-Embedder-Policy header based on database configuration.
    /// COEP is only enabled when no video URL is configured, because YouTube/Vimeo don't support COEP headers.
    /// See /coep_video.md for details.
    /// </summary>
    public class CoepMiddleware(RequestDelegate next)
    {
        public const string CacheKey = "HasVideoUrl";
        private static readonly TimeSpan s_cacheDuration = TimeSpan.FromMinutes(5);

        public async Task InvokeAsync(HttpContext context, OdbpDbContext db, IMemoryCache cache)
        {
            var hasVideoUrl = await cache.GetOrCreateAsync(CacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = s_cacheDuration;

                var resources = await db.Resources.FirstOrDefaultAsync();

                return !string.IsNullOrEmpty(resources?.VideoUrl);
            });

            if (!hasVideoUrl)
            {
                context.Response.Headers["Cross-Origin-Embedder-Policy"] = "require-corp";
            }

            await next(context);
        }
    }

    public static class CoepMiddlewareExtensions
    {
        public static IApplicationBuilder UseCoep(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CoepMiddleware>();
        }

        public static void InvalidateCoepCache(this IMemoryCache cache)
        {
            cache.Remove(CoepMiddleware.CacheKey);
        }
    }
}
