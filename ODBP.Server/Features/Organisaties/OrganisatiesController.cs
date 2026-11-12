using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ODBP.Apis.Odrc;

namespace ODBP.Features.Organisaties
{
    [ApiController]
    public class OrganisatiesController(IOdrcClientFactory clientFactory) : ControllerBase
    {
        [HttpGet("api/v2/organisaties")]
        public async Task<IActionResult> Get(CancellationToken token, [FromQuery] string? page = "1")
        {
            using var client = clientFactory.Create("Organisaties ophalen");

            var url = QueryHelpers.AddQueryString("/api/v2/organisaties", new Dictionary<string, string?>
            {
                ["page"] = page,
            });

            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(502);
            }

            var json = await response.Content.ReadFromJsonAsync<PagedResponseModel<Organisatie>>(token);

            return Ok(json);
        }
    }
}
