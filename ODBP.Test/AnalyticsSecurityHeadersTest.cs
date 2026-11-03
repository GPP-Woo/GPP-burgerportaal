using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ODBP.Config;
using ODBP.Data;
using ODBP.Data.Entities;
using ODBP.Features;

namespace ODBP.Test
{
    [TestClass]
    public class AnalyticsSecurityHeadersTest
    {
        private const string Origin = "https://matomo.example.com";

        private static void Configure(OdbpDbContext context, AnalyticsTool tool, string? url)
        {
            var resources = context.Resources.Single();
            resources.AnalyticsTool = tool;
            resources.AnalyticsUrl = url;
            context.SaveChanges();
        }

        private static string Directive(HttpResponseMessage response, string name) =>
            response.Headers.GetValues("Content-Security-Policy").Single()
                .Split(';', StringSplitOptions.TrimEntries)
                .Single(d => d.StartsWith(name + " "));

        [TestMethod]
        public async Task Zonder_analytics_bevat_de_csp_geen_analytics_origin_en_staat_coep_aan()
        {
            using var context = InMemoryDatabase.GetDbContext();
            await using var app = await TestApp.StartAsync(context);

            var response = await app.Client.GetAsync("/");

            Assert.AreEqual("script-src 'self'", Directive(response, "script-src"));
            Assert.AreEqual("require-corp", response.Headers.GetValues("Cross-Origin-Embedder-Policy").Single());
        }

        [TestMethod]
        public async Task Met_matomo_staat_alleen_de_origin_in_script_connect_en_img_src_en_is_coep_uit()
        {
            using var context = InMemoryDatabase.GetDbContext();
            Configure(context, AnalyticsTool.Matomo, $"{Origin}/matomo/");
            await using var app = await TestApp.StartAsync(context);

            var response = await app.Client.GetAsync("/");

            foreach (var directive in new[] { "script-src", "connect-src", "img-src" })
            {
                StringAssert.Contains(Directive(response, directive), $" {Origin}");
            }

            foreach (var directive in new[] { "style-src", "font-src", "frame-src", "default-src" })
            {
                Assert.IsFalse(Directive(response, directive).Contains(Origin), directive);
            }

            Assert.IsFalse(response.Headers.GetValues("Content-Security-Policy").Single().Contains($"{Origin}/"));
            Assert.IsFalse(response.Headers.Contains("Cross-Origin-Embedder-Policy"));
        }

        [TestMethod]
        public async Task Wijziging_werkt_door_na_het_legen_van_de_cache()
        {
            using var context = InMemoryDatabase.GetDbContext();
            Configure(context, AnalyticsTool.Matomo, Origin);
            await using var app = await TestApp.StartAsync(context);
            await app.Client.GetAsync("/");

            Configure(context, AnalyticsTool.None, null);
            var cached = await app.Client.GetAsync("/");
            app.Cache.InvalidateAnalyticsCache();
            var refreshed = await app.Client.GetAsync("/");

            StringAssert.Contains(Directive(cached, "script-src"), Origin);
            Assert.AreEqual("script-src 'self'", Directive(refreshed, "script-src"));
            Assert.AreEqual("require-corp", refreshed.Headers.GetValues("Cross-Origin-Embedder-Policy").Single());
        }

        [TestMethod]
        public async Task Bij_een_databasefout_geldt_de_strengste_csp_zonder_analytics()
        {
            var context = InMemoryDatabase.GetDbContext();
            Configure(context, AnalyticsTool.Matomo, Origin);
            await using var app = await TestApp.StartAsync(context);
            await app.Client.GetAsync("/");

            app.Cache.InvalidateAnalyticsCache();
            context.Dispose();
            var response = await app.Client.GetAsync("/");

            Assert.AreEqual("script-src 'self'", Directive(response, "script-src"));
            Assert.AreEqual("require-corp", response.Headers.GetValues("Cross-Origin-Embedder-Policy").Single());
        }

        [DataTestMethod]
        [DataRow("http://matomo.example.com")]
        [DataRow("https://192.168.0.1")]
        [DataRow("not a url")]
        [DataRow("https://a;sandbox;x@matomo.example.com")]
        [DataRow("https://user:pass@matomo.example.com")]
        public async Task Alleen_een_https_dns_origin_wordt_toegevoegd(string url)
        {
            using var context = InMemoryDatabase.GetDbContext();
            Configure(context, AnalyticsTool.Matomo, url);
            var middleware = new AnalyticsOriginMiddleware(_ => Task.CompletedTask, NullLogger<AnalyticsOriginMiddleware>.Instance);
            var httpContext = new DefaultHttpContext();

            await middleware.InvokeAsync(httpContext, context, new MemoryCache(new MemoryCacheOptions()), new ConfigurationBuilder().Build());

            Assert.IsNull(AnalyticsOriginMiddleware.GetOrigin(httpContext));
        }

        [DataTestMethod]
        [DataRow("https://matomo.example.com/matomo/", "https://matomo.example.com")]
        [DataRow("https://matomo.example.com:8443/x", "https://matomo.example.com:8443")]
        public async Task Origin_bevat_alleen_host_en_niet_standaard_poort(string url, string expected)
        {
            using var context = InMemoryDatabase.GetDbContext();
            Configure(context, AnalyticsTool.Matomo, url);
            var middleware = new AnalyticsOriginMiddleware(_ => Task.CompletedTask, NullLogger<AnalyticsOriginMiddleware>.Instance);
            var httpContext = new DefaultHttpContext();

            await middleware.InvokeAsync(httpContext, context, new MemoryCache(new MemoryCacheOptions()), new ConfigurationBuilder().Build());

            Assert.AreEqual(expected, AnalyticsOriginMiddleware.GetOrigin(httpContext));
        }

        [DataTestMethod]
        [DataRow(null, 5)]
        [DataRow("", 5)]
        [DataRow("abc", 5)]
        [DataRow("0", 1)]
        [DataRow("-3", 1)]
        [DataRow("1", 1)]
        [DataRow("15", 15)]
        public void Cacheduur_is_configureerbaar_met_minimum_van_een_minuut(string? value, int expectedMinutes)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> { [AnalyticsOriginMiddleware.CacheDurationConfigKey] = value })
                .Build();

            Assert.AreEqual(TimeSpan.FromMinutes(expectedMinutes), AnalyticsOriginMiddleware.GetCacheDuration(config));
        }

        // Runs the real middleware pipeline on a local Kestrel port
        private sealed class TestApp : IAsyncDisposable
        {
            private readonly WebApplication _app;

            private TestApp(WebApplication app, HttpClient client)
            {
                _app = app;
                Client = client;
            }

            public HttpClient Client { get; }

            public IMemoryCache Cache => _app.Services.GetRequiredService<IMemoryCache>();

            public static async Task<TestApp> StartAsync(OdbpDbContext context)
            {
                var builder = WebApplication.CreateBuilder();
                builder.WebHost.UseUrls("http://127.0.0.1:0");
                builder.Logging.ClearProviders();
                builder.Services.AddSingleton(context);
                builder.Services.AddSingleton<ResourcesConfig>();
                builder.Services.AddMemoryCache();
                builder.Services.AddOdbpSecurityHeaders();

                var app = builder.Build();
                app.UseAnalyticsOrigin();
                app.UseCoep();
                app.UseOdbpSecurityHeaders();
                app.MapGet("/", () => Results.Content("<html></html>", "text/html"));

                await app.StartAsync();

                return new(app, new HttpClient { BaseAddress = new Uri(app.Urls.First()) });
            }

            public async ValueTask DisposeAsync()
            {
                Client.Dispose();
                await _app.DisposeAsync();
            }
        }
    }
}
