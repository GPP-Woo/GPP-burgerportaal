using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ODBP.Config;
using ODBP.Data.Entities;
using ODBP.Features.Beheer;

namespace ODBP.Test
{
    [TestClass]
    public class AnalyticsBeheerTest
    {
        private static List<ValidationResult> Validate(Analytics model) =>
            [.. model.Validate(new ValidationContext(model))];

        [TestMethod]
        public void Validate_matomo_met_geldige_gegevens_is_geldig()
        {
            Assert.AreEqual(0, Validate(new(AnalyticsTool.Matomo, "https://matomo.example.com", "12")).Count);
        }

        [TestMethod]
        public void Validate_matomo_met_http_url_is_ongeldig()
        {
            var errors = Validate(new(AnalyticsTool.Matomo, "http://matomo.example.com", "12"));

            Assert.AreEqual(1, errors.Count);
            CollectionAssert.Contains(errors[0].MemberNames.ToList(), nameof(Analytics.Url));
        }

        [TestMethod]
        public void Validate_matomo_met_userinfo_in_url_is_ongeldig()
        {
            var errors = Validate(new(AnalyticsTool.Matomo, "https://a;sandbox;x@matomo.example.com", "12"));

            Assert.AreEqual(1, errors.Count);
            CollectionAssert.Contains(errors[0].MemberNames.ToList(), nameof(Analytics.Url));
        }

        [TestMethod]
        [DataRow("abc")]
        [DataRow("0")]
        [DataRow("0012")]
        [DataRow("١٢")]
        public void Validate_matomo_met_ongeldig_site_id_is_ongeldig(string siteId)
        {
            var errors = Validate(new(AnalyticsTool.Matomo, "https://matomo.example.com", siteId));

            Assert.AreEqual(1, errors.Count);
            CollectionAssert.Contains(errors[0].MemberNames.ToList(), nameof(Analytics.Identifier));
        }

        [TestMethod]
        public void Validate_onbekende_tool_is_ongeldig()
        {
            Assert.AreEqual(1, Validate(new((AnalyticsTool)999, null, null)).Count);
        }

        [TestMethod]
        public async Task Put_slaat_matomo_gegevens_op()
        {
            using var context = InMemoryDatabase.GetDbContext();
            var controller = new AnalyticsBeheerController(context, new MemoryCache(new MemoryCacheOptions()));

            var result = await controller.Put(new(AnalyticsTool.Matomo, " https://matomo.example.com ", " 12 "), default);

            Assert.IsInstanceOfType<OkObjectResult>(result);
            var resources = context.Resources.Single();
            Assert.AreEqual(AnalyticsTool.Matomo, resources.AnalyticsTool);
            Assert.AreEqual("https://matomo.example.com/", resources.AnalyticsUrl);
            Assert.AreEqual("12", resources.AnalyticsIdentifier);
        }

        [TestMethod]
        public async Task Put_leegt_de_cache_van_de_analytics_origin()
        {
            using var context = InMemoryDatabase.GetDbContext();
            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.Set(AnalyticsOriginMiddleware.CacheKey, "https://oud.example.com");
            var controller = new AnalyticsBeheerController(context, cache);

            await controller.Put(new(AnalyticsTool.Matomo, "https://matomo.example.com", "12"), default);

            Assert.IsFalse(cache.TryGetValue(AnalyticsOriginMiddleware.CacheKey, out _));
        }

        [TestMethod]
        public async Task Put_met_geen_wist_koppelgegevens()
        {
            using var context = InMemoryDatabase.GetDbContext();
            var controller = new AnalyticsBeheerController(context, new MemoryCache(new MemoryCacheOptions()));
            await controller.Put(new(AnalyticsTool.Matomo, "https://matomo.example.com", "12"), default);

            await controller.Put(new(AnalyticsTool.None, "https://matomo.example.com", "12"), default);

            var resources = context.Resources.Single();
            Assert.AreEqual(AnalyticsTool.None, resources.AnalyticsTool);
            Assert.IsNull(resources.AnalyticsUrl);
            Assert.IsNull(resources.AnalyticsIdentifier);
        }
    }
}
