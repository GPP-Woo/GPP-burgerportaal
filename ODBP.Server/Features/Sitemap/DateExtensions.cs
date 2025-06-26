namespace ODBP.Features.Sitemap
{
    public static class DateExtensions
    {
        const string Iso8601FormatIdentifier = "o";

        static readonly TimeZoneInfo s_westEurope = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

        public static string ToIso8601String(this DateTimeOffset dateTimeOffset) => dateTimeOffset.ToString(Iso8601FormatIdentifier);
        public static string ToIso8601String(this DateTime dateTime) => dateTime.ToString(Iso8601FormatIdentifier);
        public static string ToIso8601String(this DateOnly date) => date.ToString(Iso8601FormatIdentifier);

        public static DateTimeOffset WithWestEuropeTime(this DateOnly dateOnly, TimeOnly timeOnly) => dateOnly.WithTime(timeOnly, s_westEurope);
        public static DateTimeOffset WithTime(this DateOnly dateOnly, TimeOnly timeOnly, TimeZoneInfo timeZone)
        {
            var dateTime = new DateTime(dateOnly, timeOnly);
            return new DateTimeOffset(dateTime, timeZone.GetUtcOffset(dateTime));
        }
    }
}
