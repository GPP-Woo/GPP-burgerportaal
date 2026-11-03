using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ODBP.Data;
using ODBP.Data.Entities;

namespace ODBP.Config
{
    /// <summary>
    /// Determines the origin of the configured analytics tool (cached, default 5 minutes, configurable via
    /// <c>ANALYTICS_CACHE_DURATION_MINUTES</c>, minimum 1) and makes it available to the CSP and COEP
    /// handling via <see cref="HttpContext.Items"/>.
    /// </summary>
    public class AnalyticsOriginMiddleware(RequestDelegate next, ILogger<AnalyticsOriginMiddleware> logger)
    {
        public const string CacheKey = "AnalyticsOrigin";
        public const string ItemsKey = "AnalyticsOrigin";
        public const string CacheDurationConfigKey = "ANALYTICS_CACHE_DURATION_MINUTES";
        public const int DefaultCacheDurationMinutes = 5;

        public async Task InvokeAsync(HttpContext context, OdbpDbContext db, IMemoryCache cache, IConfiguration config)
        {
            if (!cache.TryGetValue(CacheKey, out string? origin))
            {
                try
                {
                    var resources = await db.Resources.AsNoTracking().FirstOrDefaultAsync(context.RequestAborted);
                    origin = ToOrigin(resources);
                    cache.Set(CacheKey, origin, GetCacheDuration(config));
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    // bij een databasefout geldt de strengste CSP (geen analytics); er wordt niets gecachet, zodat het snel herstelt
                    logger.LogWarning(ex, "Could not read analytics configuration");
                    origin = null;
                }
            }

            if (origin is not null)
            {
                context.Items[ItemsKey] = origin;
            }

            await next(context);
        }

        // missing or invalid values fall back to the default; values below 1 are raised to 1
        public static TimeSpan GetCacheDuration(IConfiguration config)
        {
            var minutes = int.TryParse(config[CacheDurationConfigKey], out var m) ? m : DefaultCacheDurationMinutes;
            return TimeSpan.FromMinutes(Math.Max(1, minutes));
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
