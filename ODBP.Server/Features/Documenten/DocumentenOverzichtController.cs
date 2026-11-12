using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ODBP.Apis.Odrc;

namespace ODBP.Features.Documenten
{
    [ApiController]
    public class DocumentenOverzichtController(IOdrcClientFactory clientFactory, ILogger<DocumentenOverzichtController> logger) : ControllerBase
    {
        const string Gepubliceerd = "gepubliceerd";

        [HttpGet("api/v2/documenten")]
        public async Task<IActionResult> Get(
            [FromQuery] Guid publicatie,
            CancellationToken token,
            [FromQuery] string? sorteer = "-creatiedatum",
            [FromQuery] string? page = "1")
        {
            using var client = clientFactory.Create("Documenten ophalen");

            using var publicatieResponse = await client.GetAsync($"/api/v2/publicaties/{publicatie}", HttpCompletionOption.ResponseContentRead, token);

            if (publicatieResponse.StatusCode == HttpStatusCode.NotFound)
            {
                // publicatie niet gevonden
                logger.LogInformation("Publicatie opgevraagd maar niet gevonden, publicatieId: {PublicatieId}", publicatie);
                return NotFound();
            }

            if (!publicatieResponse.IsSuccessStatusCode)
            {
                // er gaat iets mis in ODRC
                var errorBody = await publicatieResponse.Content.ReadAsStringAsync(token);
                logger.LogError("Fout bij ophalen publicatie uit ODRC, status: {Status}, body: {body}", publicatieResponse.StatusCode, errorBody);
                return StatusCode(502);
            }

            var publicatieModel = await publicatieResponse.Content.ReadFromJsonAsync<PublicatieStatusModel>(token);

            if (publicatieModel?.Publicatiestatus != Gepubliceerd)
            {
                // bijbehorende publicatie is nog niet / niet meer gepubliceerd
                logger.LogInformation("Documenten opgevraagd maar publicatie niet gepubliceerd, publicatieId: {PublicatieId}", publicatie);
                return NotFound();
            }

            // query string URL-encoded
            var url = QueryHelpers.AddQueryString("/api/v2/documenten", new Dictionary<string, string?>
            {
                ["publicatie"] = publicatie.ToString(),
                ["publicatiestatus"] = Gepubliceerd,
                ["isGereedVoorPublicatie"] = "true",
                ["sorteer"] = sorteer,
                ["page"] = page,
            });

            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

            if (!response.IsSuccessStatusCode)
            {
                // er gaat iets mis in ODRC
                var errorBody = await response.Content.ReadAsStringAsync(token);
                logger.LogError("Fout bij ophalen documenten uit ODRC, status: {Status}, body: {body}", response.StatusCode, errorBody);
                return StatusCode(502);
            }

            var json = await response.Content.ReadFromJsonAsync<PagedResponseModel<PublicatieDocument>>(token);

            return Ok(json);
        }
    }

    internal class PublicatieStatusModel
    {
        public string? Publicatiestatus { get; init; }
    }
}
