using System.Text.RegularExpressions;

namespace ODBP.Features
{
    public partial class ResourcesConfig(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        [GeneratedRegex("^gemeente\\s+", RegexOptions.IgnoreCase)]
        private static partial Regex GemeentePrefix();

        private static readonly string[] s_validOrganisationType = [
            "gemeente",
            "provincie",
            "waterschap",
            "stichting",
            "vereniging",
            "organisatie"
        ];

        private string? GetConfig(params string[] keys) => keys.Select(k => _configuration[k])
                .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

        // True als ORGANISATIE_NAAM (nog) niet is geconfigureerd maar (legacy) GEMEENTE_NAAM wel
        private bool UseLegacyFallback => string.IsNullOrWhiteSpace(_configuration["RESOURCES:ORGANISATIE_NAAM"])
                && !string.IsNullOrWhiteSpace(_configuration["RESOURCES:GEMEENTE_NAAM"]);

        public string? PortalTitle
        {
            get
            {
                var title = GetConfig("RESOURCES:PORTAAL_TITEL");

                return !string.IsNullOrWhiteSpace(title) ? title : null;
            }
        }

        // Organisatie naam, zonder type prefix, dus "Demodam" (en niet "Gemeente Demodam")
        public string? OrganisationName
        {
            get
            {
                var name = GetConfig("RESOURCES:ORGANISATIE_NAAM", "RESOURCES:GEMEENTE_NAAM");

                if (string.IsNullOrWhiteSpace(name))
                {
                    return null;
                }

                // Fallback: strip "Gemeente " uit legacy config
                return UseLegacyFallback ? GemeentePrefix().Replace(name, "") : name;
            }
        }

        // Organisatietype met het juiste lidwoord, dus "de gemeente", "het waterschap"
        public string OrganisationLabel
        {
            get
            {
                var type = GetConfig("RESOURCES:ORGANISATIE_TYPE")?.ToLowerInvariant();

                if (type != null)
                {
                    return s_validOrganisationType.Contains(type)
                        ? $"{(type == "waterschap" ? "het" : "de")} {type}"
                        : "de organisatie";
                }
                else if (UseLegacyFallback)
                {
                    return "de gemeente";
                }

                return "de organisatie";
            }
        }
        
        public string? TokensUrl
        {    
            get
            {
                var url = GetConfig("RESOURCES:DESIGN_TOKENS_URL", "RESOURCES:GEMEENTE_DESIGN_TOKENS_URL");

                return Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri.ToString() : null;
            }
        }
        
        public string? FontSources
        {
            get
            {
                var fontSources = GetConfig("RESOURCES:WEB_FONT_SOURCES", "RESOURCES:GEMEENTE_WEB_FONT_SOURCES");

                var arr = fontSources?.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .Where(s => Uri.TryCreate(s, UriKind.Absolute, out _))
                    .ToArray();

                var sources = string.Join(" ", arr ?? []);

                return !string.IsNullOrWhiteSpace(sources) ? sources : null;
            }
        }
        public string? ThemeName
        {
            get
            {
                var name = GetConfig("RESOURCES:THEME_NAAM", "RESOURCES:GEMEENTE_THEME_NAAM");

                return !string.IsNullOrWhiteSpace(name) ? name : null;
            }
        }
        public string? MediaUrl => Uri.TryCreate(_configuration["ODRC_BASE_URL"], UriKind.Absolute, out var uri) ? new Uri(uri, "media/").ToString() : null;
    }
}
