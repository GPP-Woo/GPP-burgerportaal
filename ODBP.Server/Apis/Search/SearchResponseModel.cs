namespace ODBP.Apis.Search;

public record PaginatedSearchResults
{
    public long Count { get; init; }
    public bool Previous { get; init; }
    public bool Next { get; init; }
    public IReadOnlyCollection<SearchResult> Results { get; init; } = [];
    public Facets? Facets { get; init; }
}

public record Facets
{
    public ResultTypeBucket[] ResultTypes { get; init; } = [];
    public Bucket[] InformatieCategorieen { get; init; } = [];
    public Bucket[] Publishers { get; init; } = [];
}

public record Bucket
{
    public required string Uuid { get; init; }
    public required string Naam { get; init; }
    public required long Count { get; init; }
}

public record ResultTypeBucket
{
    public required string Naam { get; init; }
    public required long Count { get; init; }
}

public record SearchResult
{
    public string? Type { get; init; }
    public required Record Record { get; init; }
}

public record Record
{
    public string? Uuid { get; init; }
    public string? Publicatie { get; init; }
    public string? OfficieleTitel { get; init; }
    public string? VerkorteTitel { get; init; }
    public Publisher? Publisher { get; init; }
    public string? Omschrijving { get; init; }
    public DateTimeOffset? Creatiedatum { get; init; }
    public DateTimeOffset? Registratiedatum { get; init; }
    public DateTimeOffset? LaatstGewijzigdDatum { get; init; }
    public InformatieCategorie[] InformatieCategorieen { get; init; } = [];
}

public record InformatieCategorie
{
    public required string Uuid { get; init; }
    public required string Naam { get; init; }
}

public record Publisher
{
    public required string Uuid { get; init; }
    public required string Naam { get; init; }
}
