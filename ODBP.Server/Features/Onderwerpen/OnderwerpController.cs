using System.Net;
using Microsoft.AspNetCore.Mvc;
using ODBP.Apis.Odrc;

namespace ODBP.Features.Onderwerpen
{
    [ApiController]
    public class OnderwerpController(IOdrcClientFactory clientFactory) : ControllerBase
    {
        const string Gepubliceerd = "gepubliceerd";

        [HttpGet("api/v2/onderwerpen/{uuid:guid}")]
        public async Task<IActionResult> Get(Guid uuid, CancellationToken token)
        {
            using var client = clientFactory.Create("Onderwerp ophalen");

            var url = $"/api/v2/onderwerpen/{uuid}";

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
