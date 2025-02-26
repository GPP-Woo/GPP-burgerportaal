
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ODBP.Apis.Search
{
    public class SearchClient(HttpClient httpClient, ILogger<SearchClient> logger) : ISearchClient
    {
        private static readonly JsonSerializerOptions s_jsonSerializerOptions = new(JsonSerializerDefaults.Web) { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

        public async Task<PaginatedSearchResults> Search(SearchRequest request, CancellationToken token)
        {
            using var content = JsonContent.Create(request, options: s_jsonSerializerOptions);
            await content.LoadIntoBufferAsync();
            using var response = await httpClient.PostAsync("/api/v1/search", content, token);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(CancellationToken.None);
                logger.LogError("Error in search client. Status: {Status}, Body: {Body}", response.StatusCode, body);
                response.EnsureSuccessStatusCode();
            }
            var result = await response.Content.ReadFromJsonAsync<PaginatedSearchResults>(token);
            return result ?? new();
        }
    }
}
