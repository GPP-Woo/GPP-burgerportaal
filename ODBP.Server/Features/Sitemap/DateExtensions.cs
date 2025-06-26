namespace ODBP.Features.Sitemap
{
    public static class DateExtensions
    {
        const string Iso8601FormatIdentifier = "o";
        public static string ToIso8601String(this DateTimeOffset dateTimeOffset) => dateTimeOffset.ToString(Iso8601FormatIdentifier);
        public static string ToIso8601String(this DateOnly date) => date.ToString(Iso8601FormatIdentifier);
    }
}
