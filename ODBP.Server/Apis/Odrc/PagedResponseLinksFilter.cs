using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.WebUtilities;

namespace ODBP.Apis.Odrc
{
    public interface IPagedResponse
    {
        string? Next { get; set; }
        string? Previous { get; set; }
    }

    /// <summary>
    /// Rewrites upstream paging links to relative links, so no upstream details are exposed to the client.
    /// </summary>
    public class PagedResponseLinksFilter : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult { Value: IPagedResponse paged })
            {
                var request = context.HttpContext.Request;
                paged.Next = ToClientLink(paged.Next, request);
                paged.Previous = ToClientLink(paged.Previous, request);
            }
        }

        private static string? ToClientLink(string? upstreamLink, HttpRequest request)
        {
            if (!Uri.TryCreate(upstreamLink, UriKind.Absolute, out var upstream))
            {
                return null;
            }

            // the upstream omits the page parameter for the first page
            var page = QueryHelpers.ParseQuery(upstream.Query).TryGetValue("page", out var value)
                ? value.ToString()
                : "1";

            if (!int.TryParse(page, out var pageNumber) || pageNumber < 1)
            {
                return null;
            }

            // keep the client's own parameters, never anything from the upstream link
            var clientQuery = request.Query
                .Where(q => !q.Key.Equals("page", StringComparison.OrdinalIgnoreCase))
                .Prepend(new("page", pageNumber.ToString()));

            return QueryHelpers.AddQueryString(request.PathBase + request.Path, clientQuery);
        }
    }
}
