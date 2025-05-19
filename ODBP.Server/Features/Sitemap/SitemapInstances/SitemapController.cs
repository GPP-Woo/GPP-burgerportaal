using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using ODBP.Apis.Odrc;
using ODBP.Config;

namespace ODBP.Features.Sitemap.SitemapInstances
{
    [ApiController]
    [OutputCache(PolicyName = OutputCachePolicies.Sitemap)]
    public class SitemapController(IOdrcClientFactory odrcClientFactory, BaseUri baseUri, ISimpleCache cache)
    {
        const string ApiVersion = "v1";
        const string ApiRoot = $"/api/{ApiVersion}";
        const string OrganisatiesPath = $"{ApiRoot}/organisaties?pageSize=100&isActief=alle";
        const string InformatieCategorieenPath = $"{ApiRoot}/informatiecategorieen?pageSize=100";
        const string PublicatiesRoot = $"{ApiRoot}/publicaties";
        const string PublicatiesQueryPath = $"{PublicatiesRoot}?pageSize=100&sorteer=registratiedatum";
        const string DocumentenRoot = $"{ApiRoot}/documenten";
        const string DocumentenQueryPath = $"{DocumentenRoot}?pageSize=100&publicatiestatus=gepubliceerd&sorteer=creatiedatum";

        [HttpGet("/api/sitemap/{year:int}/{month:int}.xml")]
        public async Task<IActionResult> Get(int year, int month, CancellationToken token)
        {
            var model = new SitemapModel();

            if (year > DateOnly.MaxValue.Year ||
                year < DateOnly.MinValue.Year ||
                month > DateOnly.MaxValue.Month ||
                month < DateOnly.MinValue.Month)
            {
                return new DiwooXmlResult(model);
            }

            var vanaf = new DateOnly(year, month, 1);
            var tot = vanaf.AddMonths(1);

            using var odrcClient = odrcClientFactory.Create(handeling: "sitemap opbouwen");

            // de documenten bevatten alleen de uuid van de publicatie die erbij hoort
            // de publicaties bevatten alleen de uuid van de organisatie / informatiecategorieen die erbij horen
            // voor de sitemap hebben we de naam en identifier nodig van organisaties / informatiecategorieen
            // daarom halen we die los op, zodat we ze per document kunnen opzoeken
            // omdat we per jaar/maand een sitemap opbouwen, cachen we de waardelijsten zodat we die niet steeds opnieuw hoeven op te halen
            var organisatiesTask = GetCachedWaardelijstDictionary(odrcClient, OrganisatiesPath, token);
            var informatiecategorieenTask = GetCachedWaardelijstDictionary(odrcClient, InformatieCategorieenPath, token);

            var gepubliceerdePublicaties = await GetPublicatieDictionary(odrcClient, vanaf, tot, token);
            var organisaties = await organisatiesTask;
            var informatiecategorieen = await informatiecategorieenTask;

            // doorloop alle documenten
            await foreach (var item in GetAllPages(odrcClient, $"{DocumentenQueryPath}&registratiedatumVanaf={vanaf:o}&registratiedatumTot={tot:o}", token))
            {
                var document = item.Deserialize(SitemapPublicatieContext.Default.OdrcDocument);

                if (document == null)
                {
                    continue;
                }

                // we halen in eerste instantie alleen de publicaties op die aangemaakt zijn in dezelfde maand als de documenten
                // theoretisch kan een publicatie net in de maand ervoor zijn aangemaakt.
                // in dat geval halen we de publciatie alsnog op, obv de id
                if (!gepubliceerdePublicaties.TryGetValue(document.Publicatie, out var publicatie))
                {
                    publicatie = await GetPublishedPublicatie(odrcClient, document.Publicatie, token);
                    if (publicatie != null)
                    {
                        gepubliceerdePublicaties[document.Publicatie] = publicatie;
                    }
                    else
                    {
                        // een document zonder publicatie. dit zou niet moeten kunnen
                        continue;
                    }
                }

                if (publicatie.Publicatiestatus != "gepubliceerd")
                {
                    continue;
                }

                // als we de publisher niet bij de publicatie kunnen vinden, is de organisatie niet bekend of heeft deze als oorsprong "zelf_toegevoegd"
                // in al deze gevallen negeren we het document
                if (!organisaties.TryGetValue(publicatie.Publisher, out var publisher))
                {
                    continue;
                }

                model.Urls.Add(new()
                {
                    Loc = new Uri(baseUri, $"{DocumentenRoot}/{document.Uuid}/download").ToString(),
                    // we nemen zowel gegevens van het document als van de publicatie over in de sitemap
                    // het lastmod veld is input voor de crawler om te bepalen of er iets opnieuw geindexeerd moet worden
                    // de laatste wijzigingsdatum van het document / de publicatie is dus leidend
                    Lastmod = MaxDateTimeOffset(document.LaatstGewijzigdDatum, publicatie.LaatstGewijzigdDatum).ToString("o"),
                    Document = new()
                    {
                        DiWoo = new()
                        {
                            Creatiedatum = document.Creatiedatum,
                            Publisher = publisher,
                            Opsteller = publicatie.Opsteller != null && organisaties.TryGetValue(publicatie.Opsteller, out var opsteller)
                                ? opsteller
                                : null,
                            Verantwoordelijke = publicatie.Verantwoordelijke != null && organisaties.TryGetValue(publicatie.Verantwoordelijke, out var verantwoordelijke)
                                ? verantwoordelijke
                                : null,
                            Identifiers = string.IsNullOrWhiteSpace(document.Identifier) ? null : [document.Identifier],
                            Omschrijvingen = string.IsNullOrWhiteSpace(document.Omschrijving) ? null : [document.Omschrijving],
                            Titelcollectie = new()
                            {
                                OfficieleTitel = document.OfficieleTitel,
                                VerkorteTitels = string.IsNullOrWhiteSpace(document.VerkorteTitel) ? null : [document.VerkorteTitel]
                            },
                            Classificatiecollectie = new()
                            {
                                // zoek de informatiecategorieen op obv de ids die in de publicatie staan.
                                // als we er eentje niet kunnen vinden, negeren we deze
                                Informatiecategorieen =
                                    LookupValuesInDictionary(publicatie.DiWooInformatieCategorieen, informatiecategorieen)
                                    .ToArray()
                            },
                            Documenthandelingen = document.Documenthandelingen.Select(x => new Documenthandeling
                            {
                                AtTime = x.AtTime,
                                SoortHandeling = new()
                                {
                                    Value = x.SoortHandeling,
                                    Resource = !string.IsNullOrWhiteSpace(x.Identifier)
                                        ? x.Identifier
                                        : x.SoortHandeling
                                }
                            }).ToArray()
                        }
                    }
                });
            }

            return new DiwooXmlResult(model);
        }

        // we cachen de waardelijsten omdat de harvester voor elke maand een sitemap opvraagt.
        // de waardelijsten zullen vrijwel nooit wijzigen tussen de sitemaps in.
        // als dit onverhoopt toch het geval is, gebruiken we een consistente waarde over de sitemaps heen
        private ValueTask<IReadOnlyDictionary<string, ResourceWithValue>> GetCachedWaardelijstDictionary(HttpClient client, string path, CancellationToken token) =>
            cache.GetOrSetAsync(path, TimeSpan.FromMinutes(1), () => GetWaardelijstDictionary(client, path, token)!)!;


        /// <summary>
        /// Haalt een waardelijst op zodat we de waardes op basis van de id kunnen opzoeken.
        /// </summary>
        private static async Task<IReadOnlyDictionary<string, ResourceWithValue>> GetWaardelijstDictionary(HttpClient client, string path, CancellationToken token)
        {
            var result = new Dictionary<string, ResourceWithValue>();
            await foreach (var item in GetAllPages(client, path, token))
            {
                if (item.TryGetProperty("uuid"u8, out var uuidProp)
                    && item.TryGetProperty("identifier"u8, out var identifierProp)
                    && item.TryGetProperty("naam"u8, out var naamprop)
                    && item.TryGetProperty("oorsprong"u8, out var oorsprong)
                    && !oorsprong.ValueEquals("zelf_toegevoegd"u8) // zelf toegevoegde organisaties & informatiecategorieen uitfilteren
                    && uuidProp.ValueKind == JsonValueKind.String
                    && identifierProp.ValueKind == JsonValueKind.String
                    && naamprop.ValueKind == JsonValueKind.String
                    )
                {
                    result[uuidProp.GetString()!] = new() { Resource = identifierProp.GetString()!, Value = naamprop.GetString()! };
                }
            }
            return result;
        }

        /// <summary>
        /// Haalt gepubliceerde publicaties zodat we die op basis van de id kunnen opzoeken.
        /// </summary>
        private static async Task<Dictionary<string, OdrcPublicatie>> GetPublicatieDictionary(HttpClient client, DateOnly vanaf, DateOnly tot, CancellationToken token)
        {
            var result = new Dictionary<string, OdrcPublicatie>();
            await foreach (var item in GetAllPages(client, $"{PublicatiesQueryPath}&registratiedatumVanaf={vanaf:o}&registratiedatumTot={tot:o}", token))
            {
                var publicatie = item.Deserialize(SitemapPublicatieContext.Default.OdrcPublicatie);
                if (publicatie != null)
                {
                    result[publicatie.Uuid] = publicatie;
                }
            }
            return result;
        }

        private static async Task<OdrcPublicatie?> GetPublishedPublicatie(HttpClient client, string id, CancellationToken token)
        {
            using var response = await client.GetAsync($"{PublicatiesRoot}/{id}", HttpCompletionOption.ResponseHeadersRead, token);
            if (!response.IsSuccessStatusCode) return null;
            await using var stream = await response.Content.ReadAsStreamAsync(token);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: token);
            if (!doc.RootElement.TryGetProperty("publicatieStatus", out var status) || !status.ValueEquals("gepubliceerd"u8)) return null;
            return doc.RootElement.Deserialize(SitemapPublicatieContext.Default.OdrcPublicatie);
        }

        /// <summary>
        /// Doorloopt alle pagina's van een API-response en retourneert de resultaten.
        /// </summary>
        private static async IAsyncEnumerable<JsonElement> GetAllPages(HttpClient client, string url, [EnumeratorCancellation] CancellationToken token)
        {
            string? next = null;
            using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token))
            {
                response.EnsureSuccessStatusCode();
                await using var stream = await response.Content.ReadAsStreamAsync(token);
                using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: token);
                if (doc.RootElement.TryGetProperty("next"u8, out var nextProp) && nextProp.ValueKind == JsonValueKind.String)
                {
                    next = nextProp.GetString();
                }
                if (doc.RootElement.TryGetProperty("results"u8, out var resultsProp) && resultsProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in resultsProp.EnumerateArray())
                    {
                        yield return item;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(next))
            {
                await foreach (var item in GetAllPages(client, next, token))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Zoekt waardes op in een dictionary op basis van een lijst van sleutels.
        /// </summary>
        private static IEnumerable<T2> LookupValuesInDictionary<T1, T2>(IEnumerable<T1> values, IReadOnlyDictionary<T1, T2> dictionary)
        {
            foreach (var item in values)
            {
                if (dictionary.TryGetValue(item, out var result))
                {
                    yield return result;
                }
            }
        }

        /// <summary>
        /// Retourneert de maximale waarde van twee DateTimeOffsets.
        /// </summary>
        private static DateTimeOffset MaxDateTimeOffset(DateTimeOffset left, DateTimeOffset right) => left > right ? left : right;
    }

    [JsonSerializable(typeof(OdrcDocument))]
    [JsonSerializable(typeof(OdrcPublicatie))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    internal partial class SitemapPublicatieContext : JsonSerializerContext
    {

    }

    public class OdrcDocument
    {
        public required string Uuid { get; set; }
        public required string Publicatie { get; set; }
        public required string OfficieleTitel { get; set; }
        public string? VerkorteTitel { get; set; }
        public required string Identifier { get; set; }
        public required DateTimeOffset LaatstGewijzigdDatum { get; set; }
        public required IReadOnlyList<OdrcDocumentHandeling> Documenthandelingen { get; set; }
        public required string Creatiedatum { get; set; }
        public string? Omschrijving { get; set; }
    }

    public class OdrcPublicatie
    {
        public required string Uuid { get; set; }
        public required string Publisher { get; set; }
        public string? Verantwoordelijke { get; set; }
        public string? Opsteller { get; set; }
        public required DateTimeOffset LaatstGewijzigdDatum { get; set; }
        public required IReadOnlyList<string> DiWooInformatieCategorieen { get; set; }
        public required string Publicatiestatus { get; set; }
    }

    public class OdrcDocumentHandeling
    {
        public required string SoortHandeling { get; set; }
        public required string AtTime { get; set; }
        public string? Identifier { get; set; }
    }
}
