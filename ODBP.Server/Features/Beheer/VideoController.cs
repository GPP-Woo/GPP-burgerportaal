using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ODBP.Authentication;

namespace ODBP.Features.Beheer
{
    [ApiController]
    [Route("api/beheer/video")]
    [Authorize(AdminPolicy.Name)]
    public partial class VideoController(IHttpClientFactory httpClientFactory) : ControllerBase
    {
        private const string InvalidError = "De video kan niet opgehaald worden. Let op: de waarde moet een geldig YouTube of Vimeo embed URL zijn.";

        [GeneratedRegex(@"^https://(www\.)?(youtube\.com|youtube-nocookie\.com)/embed/([\w-]+)")]
        private static partial Regex YouTubeEmbedRegex();

        [GeneratedRegex(@"^https://player\.vimeo\.com/video/(\d+)")]
        private static partial Regex VimeoEmbedRegex();

        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] ValidateVideoRequest request, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(request.VideoUrl))
            {
                return Ok(new ValidateVideoResponse(true, null, null));
            }

            var result = await ValidateVideoUrlAsync(request.VideoUrl, token);

            return Ok(result);
        }

        private async Task<ValidateVideoResponse> ValidateVideoUrlAsync(string url, CancellationToken token)
        {
            var youtubeMatch = YouTubeEmbedRegex().Match(url);

            if (youtubeMatch.Success)
            {
                var videoId = youtubeMatch.Groups[3].Value;

                return await FetchOEmbedAsync($"https://www.youtube.com/oembed?url=https://www.youtube.com/watch?v={videoId}&format=json", token);
            }

            var vimeoMatch = VimeoEmbedRegex().Match(url);

            if (vimeoMatch.Success)
            {
                var videoId = vimeoMatch.Groups[1].Value;

                return await FetchOEmbedAsync($"https://vimeo.com/api/oembed.json?url=https://vimeo.com/{videoId}", token);
            }

            return ValidateVideoResponse.Failure(InvalidError);
        }

        private async Task<ValidateVideoResponse> FetchOEmbedAsync(string oembedUrl, CancellationToken token)
        {
            try
            {
                var client = httpClientFactory.CreateClient();

                var response = await client.GetAsync(oembedUrl, token);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<OEmbedResponse>(token);

                    return ValidateVideoResponse.Succes(json?.Title);
                }
            }
            catch { }

            return ValidateVideoResponse.Failure(InvalidError);
        }

        private record OEmbedResponse(string? Title);
    }

    public record ValidateVideoRequest(string? VideoUrl);

    public record ValidateVideoResponse(bool Valid, string? Title, string? Error)
    {
        public static ValidateVideoResponse Succes(string? title) => new(true, title, null);
        public static ValidateVideoResponse Failure(string error) => new(false, null, error);
    }
}
