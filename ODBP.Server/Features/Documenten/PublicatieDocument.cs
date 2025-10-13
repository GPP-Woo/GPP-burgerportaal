namespace ODBP.Features.Documenten
{
    public class PublicatieDocument
    {
        public Guid Uuid { get; set; }
        public Guid Publicatie { get; set; }
        public required string OfficieleTitel { get; set; }
        public string? VerkorteTitel { get; set; }
        public string? Omschrijving { get; set; }
        public string? Publicatiestatus { get; set; }
        public DateOnly Creatiedatum { get; set; }
        public DateTimeOffset? GepubliceerdOp { get; init; }
        public DateTimeOffset LaatstGewijzigdDatum { get; set; }
        public DateTimeOffset? Ontvangstdatum { get; set; }
        public DateTimeOffset? DatumOndertekend { get; set; }
        public required string Bestandsnaam { get; set; }
        public required double Bestandsomvang { get; set; }
        public List<Identifier>? Kenmerken { get; set; }
    }

    public class Identifier
    {
        public string? Kenmerk { get; set; }
        public string? Bron { get; set; }
    }
}
