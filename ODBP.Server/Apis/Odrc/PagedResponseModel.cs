namespace ODBP.Apis.Odrc
{
    public class PagedResponseModel<T> : IPagedResponse
    {
        public required IReadOnlyList<T> Results { get; set; }
        public int Count { get; set; }
        public string? Next { get; set; }
        public string? Previous { get; set; }
    }
}
