using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ODBP.Apis.Odrc;

namespace ODBP.Features.Sitemap.SitemapIndex
{
    [ApiController]
    public class SitemapIndexController(BaseUri baseUri, IOdrcClientFactory odrcClientFactory)
    {
        [HttpGet(ApiRoutes.SitemapIndex)]
        public async Task<IActionResult> Get(CancellationToken token)
        {
            var result = new SitemapIndexModel { Sitemaps = [] };
            var startDate = await GetEarliestDocumentCreationDate(token);
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

        private async Task<DateTimeOffset?> GetEarliestDocumentCreationDate(CancellationToken token)
        {
            const string Url = $"/api/v1/documenten?publicatiestatus=gepubliceerd&pageSize=10&sorteer=creatiedatum";
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
