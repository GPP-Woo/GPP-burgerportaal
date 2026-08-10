using Microsoft.Extensions.Logging.Abstractions;
using ODBP.Features.Sitemap;
using ODBP.Features.Sitemap.SitemapInstances;
using Xunit;

namespace ODBP.Tests.Features.Sitemap
{
    public class SitemapControllerTests
    {
        private const string OrganisatieUuid = "11111111-1111-1111-1111-111111111111";
        private const string CategorieUuid = "22222222-2222-2222-2222-222222222222";
        private const string PublicatieUuid = "33333333-3333-3333-3333-333333333333";
        private const string GoedDocumentUuid = "44444444-4444-4444-4444-444444444444";
        private const string DocumentZonderPublicatieUuid = "55555555-5555-5555-5555-555555555555";

        /// <summary>
        /// Regressietest: een document waarvan "publicatie" null is liet de hele
        /// maand-sitemap omvallen met een 500. TryGetValue gooit namelijk een
        /// ArgumentNullException op een null key, dus één zo'n document nam alle
        /// andere documenten mee. Het document moet overgeslagen worden.
        /// </summary>
        [Fact]
        public async Task Document_zonder_publicatie_laat_de_sitemap_niet_omvallen()
        {
            var result = await GetSitemap(DocumentenJson(
                DocumentJson(GoedDocumentUuid, publicatie: $"\"{PublicatieUuid}\""),
                DocumentJson(DocumentZonderPublicatieUuid, publicatie: "null")));

            var locs = result.Model.Urls.Select(x => x.Loc).ToArray();

            Assert.Contains(locs, loc => loc.Contains(GoedDocumentUuid, StringComparison.Ordinal));
            Assert.DoesNotContain(locs, loc => loc.Contains(DocumentZonderPublicatieUuid, StringComparison.Ordinal));
        }

        /// <summary>Een ontbrekend "publicatie"-veld levert net als null een overgeslagen document op.</summary>
        [Fact]
        public async Task Document_zonder_publicatie_veld_wordt_overgeslagen()
        {
            var result = await GetSitemap(DocumentenJson(
                $$"""
                { "uuid": "{{DocumentZonderPublicatieUuid}}", "officieleTitel": "Geen publicatie", "identifier": "id-1",
                  "laatstGewijzigdDatum": "2026-08-10T12:00:00+02:00", "creatiedatum": "2026-08-10" }
                """));

            Assert.Empty(result.Model.Urls);
        }

        /// <summary>Het gelukkige pad, zodat de test hierboven niet vacuüm groen staat.</summary>
        [Fact]
        public async Task Gepubliceerd_document_komt_met_zijn_metadata_in_de_sitemap()
        {
            var result = await GetSitemap(DocumentenJson(
                DocumentJson(GoedDocumentUuid, publicatie: $"\"{PublicatieUuid}\"")));

            var url = Assert.Single(result.Model.Urls);
            Assert.Equal($"http://burgerportaal.test/api/v2/documenten/{GoedDocumentUuid}/download", url.Loc);
            Assert.Equal("gemeente Testdorp", url.Document.DiWoo.Publisher.Value);
            Assert.Equal("E2E titel", url.Document.DiWoo.Titelcollectie.OfficieleTitel);
        }

        /// <summary>Een document waarvan de publicatie onbekend is hoort er ook niet in.</summary>
        [Fact]
        public async Task Document_met_onbekende_publicatie_wordt_overgeslagen()
        {
            var result = await GetSitemap(DocumentenJson(
                DocumentJson(GoedDocumentUuid, publicatie: "\"99999999-9999-9999-9999-999999999999\"")));

            Assert.Empty(result.Model.Urls);
        }

        private static async Task<XmlResult<SitemapModel>> GetSitemap(string documentenJson)
        {
            var controller = new SitemapController(
                new FakeOdrcClientFactory(new Dictionary<string, string>
                {
                    ["/api/v2/organisaties"] = WaardelijstJson(OrganisatieUuid, "gemeente Testdorp", "https://identifier.overheid.nl/tooi/id/gemeente/gm0293"),
                    ["/api/v2/informatiecategorieen"] = WaardelijstJson(CategorieUuid, "Woo-verzoeken", "https://identifier.overheid.nl/tooi/def/thes/kern/c_9d8f9b7d"),
                    ["/api/v2/publicaties"] = PublicatiesJson(),
                    ["/api/v2/documenten"] = documentenJson,
                }),
                new BaseUri(new Uri("http://burgerportaal.test")),
                new NoCache(),
                NullLogger<SitemapController>.Instance);

            var result = await controller.Get(2026, 8, CancellationToken.None);

            // Een 500 zou hier als exception uit Get komen, niet als resultaat.
            return Assert.IsType<DiwooXmlResult>(result);
        }

        private static string WaardelijstJson(string uuid, string naam, string identifier) =>
            $$"""
            { "next": null, "results": [
              { "uuid": "{{uuid}}", "naam": "{{naam}}", "identifier": "{{identifier}}", "oorsprong": "waardelijst" }
            ] }
            """;

        private static string PublicatiesJson() =>
            $$"""
            { "next": null, "results": [
              { "uuid": "{{PublicatieUuid}}", "publisher": "{{OrganisatieUuid}}", "verantwoordelijke": "{{OrganisatieUuid}}",
                "publicatiestatus": "gepubliceerd", "laatstGewijzigdDatum": "2026-08-10T12:00:00+02:00",
                "diWooInformatieCategorieen": ["{{CategorieUuid}}"] }
            ] }
            """;

        private static string DocumentenJson(params string[] documenten) =>
            $$"""
            { "next": null, "results": [ {{string.Join(",", documenten)}} ] }
            """;

        private static string DocumentJson(string uuid, string publicatie) =>
            $$"""
            { "uuid": "{{uuid}}", "publicatie": {{publicatie}}, "officieleTitel": "E2E titel", "identifier": "id-1",
              "laatstGewijzigdDatum": "2026-08-10T12:00:00+02:00", "creatiedatum": "2026-08-10" }
            """;
    }
}
