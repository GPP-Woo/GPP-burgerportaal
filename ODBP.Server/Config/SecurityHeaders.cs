using ODBP.Features;

namespace Microsoft.AspNetCore.Builder
{
    public static class SecurityHeaders
    {
        public static IApplicationBuilder UseOdbpSecurityHeaders(this WebApplication app)
        {
            var resourcesConfig = app.Services.GetRequiredService<ResourcesConfig>();

            var connectSources = new List<string?> {
                "'self'"
            };

            var styleSources = new List<string?> {
                "'self'",
                resourcesConfig.TokensUrl
            };

            var imgSources = new List<string?> {
                "'self'",
                resourcesConfig.FaviconUrl,
                resourcesConfig.ImageUrl,
                resourcesConfig.MediaUrl
            };

            var fontSources = new List<string?> {
                "'self'",
                resourcesConfig.FontSources
            };

            var frameSources = new List<string?> {
                "'self'",
                resourcesConfig.VideoUrl
            };

            // Add svg logo to connectSources to be able to fetch through js
            var logoUrl = resourcesConfig.LogoUrl;

            if (!string.IsNullOrEmpty(logoUrl))
            {
                if (logoUrl.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                {
                    connectSources.Add(logoUrl);
                }
                else
                {
                    imgSources.Add(logoUrl);
                }
            }

            // Add video cdn's to img-src: during playback the player
            // may load additional images which are then subject to our parent CSP
            var videoCdnDomains = new[] { "*.ytimg.com", "*.vimeocdn.com" };

            if (!string.IsNullOrEmpty(resourcesConfig.VideoUrl))
            {
                imgSources.AddRange(videoCdnDomains);
            }

            return app.UseSecurityHeaders(headerPolicyCollection =>
            {
                headerPolicyCollection
                    .AddDefaultSecurityHeaders()
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
                        csp.AddScriptSrc().Self();
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
                        permissions.AddAccelerometer().None();
                        permissions.AddAmbientLightSensor().None();
                        permissions.AddAutoplay().None();
                        permissions.AddCamera().None();
                        permissions.AddEncryptedMedia().None();
                        permissions.AddFullscreen();
                        permissions.AddGeolocation().None();
                        permissions.AddGyroscope().None();
                        permissions.AddMagnetometer().None();
                        permissions.AddMicrophone().None();
                        permissions.AddMidi().None();
                        permissions.AddPayment().None();
                        permissions.AddPictureInPicture().None();
                        permissions.AddSpeaker().None();
                        permissions.AddSyncXHR().None();
                        permissions.AddUsb().None();
                        permissions.AddVR().None();
                        
                        if (!string.IsNullOrEmpty(resourcesConfig.VideoUrl))
                        {
                            // Origin needs to be quoted, browser requirement
                            var videoOrigin = $"\"{ new Uri(resourcesConfig.VideoUrl).GetLeftPart(UriPartial.Authority) }\"";

                            // Enable these for video
                            permissions.AddAccelerometer().Self().Sources.Add(videoOrigin);
                            permissions.AddEncryptedMedia().Self().Sources.Add(videoOrigin);
                            permissions.AddFullscreen().Self().Sources.Add(videoOrigin);
                            permissions.AddGyroscope().Self().Sources.Add(videoOrigin);
                            permissions.AddPictureInPicture().Self().Sources.Add(videoOrigin);
                        }
                    });

                // COEP disabled for video embedding - see /coep_video.md
                // YouTube/Vimeo don't provide required COEP headers on iframe sources
                if (string.IsNullOrEmpty(resourcesConfig.VideoUrl))
                {
                    headerPolicyCollection
                        .AddCrossOriginEmbedderPolicy(x =>
                        {
                            x.RequireCorp();
                        });
                }
            });
        }
    }
}
