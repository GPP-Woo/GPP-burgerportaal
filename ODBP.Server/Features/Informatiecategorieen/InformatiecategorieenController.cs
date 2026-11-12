using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ODBP.Apis.Odrc;

namespace ODBP.Features.Informatiecategorieen
{
    [ApiController]
    public class InformatiecategorieenController(IOdrcClientFactory clientFactory) : ControllerBase
    {
        [HttpGet("api/v2/informatiecategorieen")]
        public async Task<IActionResult> Get(CancellationToken token, [FromQuery] string? page = "1")
        {
            using var client = clientFactory.Create("Informatiecategorieen ophalen");

            var url = QueryHelpers.AddQueryString("/api/v2/informatiecategorieen", new Dictionary<string, string?>
            {
                ["page"] = page,
            });

            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(502);
            }

            var json = await response.Content.ReadFromJsonAsync<PagedResponseModel<Informatiecategorie>>(token);

            return Ok(json);
        }
    }
}
