using System.Net;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using ODBP.Apis.Odrc;

namespace ODBP.Features.Onderwerpen
{
    [ApiController]
    public class OnderwerpenController(IOdrcClientFactory clientFactory) : ControllerBase
    {
        [HttpGet("api/{version}/onderwerpen")]
        public async Task<IActionResult> Get(string version, CancellationToken token, [FromQuery] string? page = "1")
        {
            using var client = clientFactory.Create("Onderwerpen ophalen");
            var url = $"/api/{version}/onderwerpen?page={page}";

            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(502);
            }

            var json = await response.Content.ReadFromJsonAsync<PagedResponseModel<JsonNode>>(token);

            return Ok(json);
        }
    }
}
