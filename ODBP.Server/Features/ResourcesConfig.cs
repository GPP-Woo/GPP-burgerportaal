using System.Text;

namespace ODBP.Features
{
    public class ResourcesConfig(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public string? Title
        {
            get
            {
                var title = _configuration["RESOURCES:PORTAAL_TITEL"];
                return !string.IsNullOrWhiteSpace(title) ? title : null;
            }
        }
        public string? Name
        {
            get
            {
                var name = _configuration["RESOURCES:GEMEENTE_NAAM"];
                return !string.IsNullOrWhiteSpace(name) ? name : null;
            }
        }
        public string? LogoUrl => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_LOGO_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? FaviconUrl => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_FAVICON_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? ImageUrl => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_MAIN_IMAGE_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? TokensUrl => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_DESIGN_TOKENS_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? FontSources
        {
            get
            {
                var arr = _configuration["RESOURCES:GEMEENTE_WEB_FONT_SOURCES"]?.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .Where(s => Uri.TryCreate(s, UriKind.Absolute, out _))
                    .ToArray();

                var sources = string.Join(" ", arr ?? []);

                return !string.IsNullOrWhiteSpace(sources) ? sources : null;
            }
        }
        public string? Theme
        {
            get
            {
                var theme = _configuration["RESOURCES:GEMEENTE_THEME_NAAM"];
                return !string.IsNullOrWhiteSpace(theme) ? theme : null;
            }
        }
        public string? MediaUrl => Uri.TryCreate(_configuration["ODRC_BASE_URL"], UriKind.Absolute, out var uri) ? new Uri(uri, "media/").ToString() : null;
        public string? WebsiteUrl => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_WEBSITE_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? PrivacyUrl => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_PRIVACY_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? ContactUrl => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_CONTACT_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? A11yUrl => Uri.TryCreate(_configuration["RESOURCES:TOEGANKELIJKHEIDSVERKLARING_REGISTER_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
    }
}
