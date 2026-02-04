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

                // COEP is handled dynamically by CoepMiddleware based on database
                // See /coep_video.md for details
            });
        }
    }
}
