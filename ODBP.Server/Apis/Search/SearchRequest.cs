namespace ODBP.Apis.Search
{
    public record SearchRequest
    {
        public string? Query { get; init; }
        public int? Page { get; init; }
        public int? PageSize { get; init; }
        public string? Sort { get; init; }
        public DateTimeOffset? RegistratiedatumVanaf { get; init; }
        public DateTimeOffset? RegistratiedatumTot { get; init; }
        public DateTimeOffset? GepubliceerdOpVanaf { get; init; }
        public DateTimeOffset? GepubliceerdOpTot { get; init; }
        public DateTimeOffset? LaatstGewijzigdDatumVanaf { get; init; }
        public DateTimeOffset? LaatstGewijzigdDatumTot { get; init; }
        public string[]? ResultTypes { get; init; }
        public string[]? Publishers { get; init; }
        public string[]? InformatieCategorieen { get; init; }
        public string[]? Onderwerpen { get; init; }
    }
}
