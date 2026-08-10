using System.Net;
using ODBP.Apis.Odrc;
using ODBP.Features.Sitemap;

namespace ODBP.Tests.Features.Sitemap
{
    /// <summary>
    /// Antwoordt met vaste json per api-pad, zodat een controller getest kan worden
    /// zonder een echte ODRC. Matcht op het begin van het pad (de controller hangt
    /// query-parameters aan de urls).
    /// </summary>
    internal sealed class FakeOdrcHandler(IReadOnlyDictionary<string, string> responsesByPath) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var path = request.RequestUri!.AbsolutePath;
            var body = responsesByPath.FirstOrDefault(x => path.StartsWith(x.Key, StringComparison.Ordinal)).Value;

            return Task.FromResult(body == null
                ? new HttpResponseMessage(HttpStatusCode.NotFound)
                : new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json")
                });
        }
    }

    internal sealed class FakeOdrcClientFactory(IReadOnlyDictionary<string, string> responsesByPath) : IOdrcClientFactory
    {
        public HttpClient Create(string handeling) => new(new FakeOdrcHandler(responsesByPath))
        {
            BaseAddress = new Uri("http://odrc.test")
        };
    }

    /// <summary>Geen cache: elke aanroep gaat door naar de factory, zodat tests elkaar niet beïnvloeden.</summary>
    internal sealed class NoCache : ISimpleCache
    {
        public async ValueTask<TItem> GetOrSetAsync<TItem>(string key, TimeSpan absoluteExpirationRelativeToNow, Func<Task<TItem>> factory) =>
            await factory();
    }
}
