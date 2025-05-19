using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ODBP.Apis.Odrc;

namespace ODBP.Features.Sitemap.SitemapIndex
{
    [ApiController]
    public class SitemapIndexController(BaseUri baseUri, IOdrcClientFactory odrcClientFactory, ISimpleCache cache)
    {
        /// <summary>
        /// Retourneert voor elke maand, vanaf de eerste documentcreatiedatum tot en met nu, een link naar de betreffende sitemap. Niet elke maand heeft per definitie documenten: die sitemaps zijn leeg (maar wel valide)
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet(ApiRoutes.SitemapIndex)]
        public async Task<IActionResult> Get(CancellationToken token)
        {
            var result = new SitemapIndexModel();
            var startDate = await GetCachedEarliestDocumentCreationDate(token);
            if (startDate == null) return new XmlResult<SitemapIndexModel>(result);
            var now = DateTimeOffset.UtcNow;
            var sitemaps = GetAllMonthsBetweenInclusive(DateOnly.FromDateTime(startDate.Value.Date), DateOnly.FromDateTime(now.Date))
                .Select(x => new SitemapLink
                {
                    Loc = new Uri(baseUri, $"/api/sitemap/{x.Year}/{x.Month}.xml").ToString()
                });
            result.Sitemaps.AddRange(sitemaps);
            return new XmlResult<SitemapIndexModel>(result);
        }

        // we kunnen de vroegste creatiedatum theoretisch permanent cachen als we er eentje hebben, want er gaat nooit een document eerder aangemaakt worden
        private ValueTask<DateTimeOffset?> GetCachedEarliestDocumentCreationDate(CancellationToken token) =>
            cache.GetOrSetAsync(nameof(GetEarliestDocumentCreationDate), TimeSpan.MaxValue, () => GetEarliestDocumentCreationDate(token));

        private async Task<DateTimeOffset?> GetEarliestDocumentCreationDate(CancellationToken token)
        {
            const string Url = $"/api/v1/documenten?publicatiestatus=gepubliceerd&pageSize=1&sorteer=creatiedatum";
            using var client = odrcClientFactory.Create("Opbouwen sitemapindex");
            using var response = await client.GetAsync(Url, HttpCompletionOption.ResponseHeadersRead, token);
            if (!response.IsSuccessStatusCode) return null;
            using var stream = await response.Content.ReadAsStreamAsync(token);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: token);
            return doc.RootElement
                .GetProperty("results")
                .EnumerateArray()
                .Select(x => x
                    .GetProperty("creatiedatum")
                    .GetDateTimeOffset())
                .Cast<DateTimeOffset?>()
                .FirstOrDefault();
        }

        private readonly record struct YearMonth(int Year, int Month);
        private static IEnumerable<YearMonth> GetAllMonthsBetweenInclusive(DateOnly left, DateOnly right)
        {
            var (min, max) = left > right
                ? (right, left)
                : (left, right);

            for (var year = min.Year; year <= max.Year; year++)
            {
                var startMonth = year == min.Year
                    ? min.Month
                    : 1;

                var endMonth = year == max.Year
                    ? max.Month
                    : 12;

                for (var month = startMonth; month <= endMonth; month++)
                {
                    yield return new YearMonth(year, month);
                }
            }
        }
    }
}
