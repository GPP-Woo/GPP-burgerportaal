using System.Net;
using Microsoft.AspNetCore.Mvc;
using ODBP.Apis.Odrc;

namespace ODBP.Features.Onderwerpen
{
    [ApiController]
    public class OnderwerpController(IOdrcClientFactory clientFactory) : ControllerBase
    {
        const string Gepubliceerd = "gepubliceerd";

        [HttpGet("api/{version}/onderwerpen/{uuid:guid}")]
        public async Task<IActionResult> Get(string version, Guid uuid, CancellationToken token)
        {
            using var client = clientFactory.Create("Onderwerp ophalen");

            var url = $"/api/{version}/onderwerpen/{uuid}";

            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(502);
            }

            var onderwerp = await response.Content.ReadFromJsonAsync<Onderwerp>(token);

            return onderwerp?.Publicatiestatus != Gepubliceerd ? NotFound() : Ok(onderwerp);
        }
    }
}
