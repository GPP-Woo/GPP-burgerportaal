using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ODBP.Data;
using ODBP.Data.Entities;

namespace ODBP.Config
{
    /// <summary>
    /// Determines the origin of the configured analytics tool (cached for a maximum of 5 minutes)
    /// and makes it available to the CSP and COEP handling via <see cref="HttpContext.Items"/>.
    /// </summary>
    public class AnalyticsOriginMiddleware(RequestDelegate next, ILogger<AnalyticsOriginMiddleware> logger)
    {
        public const string CacheKey = "AnalyticsOrigin";
        private const string LastKnownCacheKey = "AnalyticsOriginLastKnown";
        public const string ItemsKey = "AnalyticsOrigin";
        private static readonly TimeSpan s_cacheDuration = TimeSpan.FromMinutes(5);

        public async Task InvokeAsync(HttpContext context, OdbpDbContext db, IMemoryCache cache)
        {
            if (!cache.TryGetValue(CacheKey, out string? origin))
            {
                try
                {
                    var resources = await db.Resources.AsNoTracking().FirstOrDefaultAsync(context.RequestAborted);
                    origin = ToOrigin(resources);
                    cache.Set(LastKnownCacheKey, origin);
                    cache.Set(CacheKey, origin, s_cacheDuration);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    // bij een databasefout blijft de laatst bekende waarde gelden, anders geen analytics
                    logger.LogWarning(ex, "Could not read analytics configuration");
                    cache.TryGetValue(LastKnownCacheKey, out origin);
                }
            }

            if (origin is not null)
            {
                context.Items[ItemsKey] = origin;
            }

            await next(context);
        }

        public static string? GetOrigin(HttpContext context) =>
            context.Items.TryGetValue(ItemsKey, out var value) ? value as string : null;

        // only a plain https origin (no path or wildcard) is ever placed in the CSP
        private static string? ToOrigin(Resources? resources)
        {
            if (resources is null || resources.AnalyticsTool == AnalyticsTool.None)
            {
                return null;
            }

            if (!Uri.TryCreate(resources.AnalyticsUrl, UriKind.Absolute, out var uri)
                || uri.Scheme != Uri.UriSchemeHttps
                || uri.HostNameType != UriHostNameType.Dns)
            {
                return null;
            }

            return uri.GetLeftPart(UriPartial.Authority);
        }
    }

    public static class AnalyticsOriginMiddlewareExtensions
    {
        public static IApplicationBuilder UseAnalyticsOrigin(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AnalyticsOriginMiddleware>();
        }

        public static void InvalidateAnalyticsCache(this IMemoryCache cache)
        {
            cache.Remove(AnalyticsOriginMiddleware.CacheKey);
        }
    }
}
