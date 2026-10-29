using System.Collections.Concurrent;
using ODBP.Features;

namespace ODBP.Config
{
    public static class SecurityHeaders
    {
        // Video embed domains - always allowed in CSP for dynamic video configuration
        private static readonly string[] s_videoEmbedDomains = [
            "https://youtube.com",
            "https://www.youtube.com",
            "https://youtube-nocookie.com",
            "https://www.youtube-nocookie.com",
            "https://player.vimeo.com"
        ];
        private static readonly string[] s_videoCdnDomains = ["*.ytimg.com", "*.vimeocdn.com"];
        private static readonly string[] s_videoOrigins = [
            "\"https://youtube.com\"",
            "\"https://www.youtube.com\"",
            "\"https://youtube-nocookie.com\"",
            "\"https://www.youtube-nocookie.com\"",
            "\"https://player.vimeo.com\""
        ];

        public static IServiceCollection AddOdbpSecurityHeaders(this IServiceCollection services)
        {
            services.AddSingleton<OdbpSecurityHeaderPolicies>();

            // the policy is selected per request, because the CSP depends on the analytics configuration
            services.AddSecurityHeaderPolicies()
                .SetPolicySelector(context => context.HttpContext.RequestServices
                    .GetRequiredService<OdbpSecurityHeaderPolicies>()
                    .Get(AnalyticsOriginMiddleware.GetOrigin(context.HttpContext)));

            return services;
        }

        public static IApplicationBuilder UseOdbpSecurityHeaders(this WebApplication app) => app.UseSecurityHeaders();

        internal static HeaderPolicyCollection BuildPolicies(ResourcesConfig resourcesConfig, string? analyticsOrigin)
        {
            var connectSources = new List<string?> {
                "'self'"
            };

            var styleSources = new List<string?> {
                "'self'",
                resourcesConfig.TokensUrl
            };

            var imgSources = new List<string?> {
                "'self'",
                resourcesConfig.MediaUrl
            };

            imgSources.AddRange(s_videoCdnDomains);

            var fontSources = new List<string?> {
                "'self'",
                resourcesConfig.FontSources
            };

            var frameSources = new List<string?> {
                "'self'"
            };

            frameSources.AddRange(s_videoEmbedDomains);

            // the analytics origin only widens script-src, connect-src and img-src
            var scriptSources = new List<string?> { "'self'" };

            if (analyticsOrigin is not null)
            {
                scriptSources.Add(analyticsOrigin);
                connectSources.Add(analyticsOrigin);
                imgSources.Add(analyticsOrigin);
            }

            var headerPolicyCollection = new HeaderPolicyCollection();

            headerPolicyCollection
                // explicit instead of AddDefaultSecurityHeaders(), whose defaults differ per package version
                // (COEP is handled dynamically by CoepMiddleware based on database, see /coep_video.md)
                .AddFrameOptionsDeny()
                .AddContentTypeOptionsNoSniff()
                .AddReferrerPolicyStrictOriginWhenCrossOrigin()
                .AddStrictTransportSecurityMaxAgeIncludeSubDomains()
                .AddCrossOriginOpenerPolicy(x =>
                {
                    x.SameOrigin();
                })
                .AddCrossOriginResourcePolicy(x =>
                {
                    x.SameOrigin();
                })
                .AddContentSecurityPolicy(csp =>
                {
                    csp.AddUpgradeInsecureRequests();
                    csp.AddDefaultSrc().None();
                    csp.AddConnectSrc().From(string.Join(" ", connectSources.Where(src => !string.IsNullOrWhiteSpace(src))));
                    csp.AddScriptSrc().From(string.Join(" ", scriptSources));
                    csp.AddStyleSrc().From(string.Join(" ", styleSources.Where(src => !string.IsNullOrWhiteSpace(src))));
                    csp.AddImgSrc().From(string.Join(" ", imgSources.Where(src => !string.IsNullOrWhiteSpace(src))));
                    csp.AddFontSrc().From(string.Join(" ", fontSources.Where(src => !string.IsNullOrWhiteSpace(src))));
                    csp.AddFrameSrc().From(string.Join(" ", frameSources.Where(src => !string.IsNullOrWhiteSpace(src))));
                    csp.AddFrameAncestors().None();
                    csp.AddFormAction().Self();
                    csp.AddBaseUri().None();
                })
                .AddPermissionsPolicy(permissions =>
                {
                    // permissions.AddAccelerometer().None();
                    permissions.AddAmbientLightSensor().None();
                    permissions.AddAutoplay().None();
                    permissions.AddCamera().None();
                    //permissions.AddEncryptedMedia().None();
                    //permissions.AddFullscreen();
                    permissions.AddGeolocation().None();
                    // permissions.AddGyroscope().None();
                    permissions.AddMagnetometer().None();
                    permissions.AddMicrophone().None();
                    permissions.AddMidi().None();
                    permissions.AddPayment().None();
                    // permissions.AddPictureInPicture().None();
                    permissions.AddSpeaker().None();
                    permissions.AddSyncXHR().None();
                    permissions.AddUsb().None();
                    permissions.AddVR().None();

                    var accelerometer = permissions.AddAccelerometer().Self();
                    var encryptedMedia = permissions.AddEncryptedMedia().Self();
                    var fullscreen = permissions.AddFullscreen().Self();
                    var gyroscope = permissions.AddGyroscope().Self();
                    var pictureInPicture = permissions.AddPictureInPicture().Self();

                    foreach (var origin in s_videoOrigins)
                    {
                        accelerometer.Sources.Add(origin);
                        encryptedMedia.Sources.Add(origin);
                        fullscreen.Sources.Add(origin);
                        gyroscope.Sources.Add(origin);
                        pictureInPicture.Sources.Add(origin);
                    }
                });

            return headerPolicyCollection;
        }
    }

    /// <summary>
    /// Holds a header policy per analytics origin, so the policies are not rebuilt on every request.
    /// </summary>
    public class OdbpSecurityHeaderPolicies(ResourcesConfig resourcesConfig)
    {
        private readonly ConcurrentDictionary<string, HeaderPolicyCollection> _policies = new();

        public HeaderPolicyCollection Get(string? analyticsOrigin) =>
            _policies.GetOrAdd(analyticsOrigin ?? "", _ => SecurityHeaders.BuildPolicies(resourcesConfig, analyticsOrigin));
    }
}
