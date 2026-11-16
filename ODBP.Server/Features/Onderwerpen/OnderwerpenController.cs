using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ODBP.Apis.Odrc;

namespace ODBP.Features.Onderwerpen
{
    [ApiController]
    public class OnderwerpenController(IOdrcClientFactory clientFactory, ILogger<OnderwerpenController> logger) : ControllerBase
    {
        const string Gepubliceerd = "gepubliceerd";

        [HttpGet("api/v2/onderwerpen")]
        public async Task<IActionResult> Get(CancellationToken token, [FromQuery] string? page = "1")
        {
            using var client = clientFactory.Create("Onderwerpen ophalen");

            // query string URL-encoded
            var url = QueryHelpers.AddQueryString("/api/v2/onderwerpen", new Dictionary<string, string?>
            {
                ["page"] = page,
                ["publicatiestatus"] = Gepubliceerd,
            });

            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

            if (!response.IsSuccessStatusCode)
            {
                // er gaat iets mis in ODRC
                var errorBody = await response.Content.ReadAsStringAsync(token);
                logger.LogError("Fout bij ophalen onderwerpen uit ODRC, status: {Status}, body: {body}", response.StatusCode, errorBody);
                return StatusCode(502);
            }

            var json = await response.Content.ReadFromJsonAsync<PagedResponseModel<Onderwerp>>(token);

            return Ok(json);
        }
    }
}
