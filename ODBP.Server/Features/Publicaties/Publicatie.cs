namespace ODBP.Features.Publicaties
{
    public class Publicatie
    {
        public Guid Uuid { get; set; }
        public string? Publisher { get; set; }
        public string? OfficieleTitel { get; set; }
        public string? VerkorteTitel { get; set; }
        public string? Omschrijving { get; set; }
        public string? Publicatiestatus { get; set; }
        public DateTimeOffset? GepubliceerdOp { get; set; }
        public DateTimeOffset LaatstGewijzigdDatum { get; set; }
        public DateOnly? DatumBeginGeldigheid { get; set; }
        public DateOnly? DatumEindeGeldigheid { get; set; }
        public List<string>? InformatieCategorieen { get; set; }
        public List<string>? Onderwerpen { get; set; }
        public List<Identifier>? Kenmerken { get; set; }
        public InzageProcedure? InzageProcedure { get; set; }
    }

    public class Identifier
    {
        public string? Kenmerk { get; set; }
        public string? Bron { get; set; }
    }

    public class InzageProcedure
    {
        public Guid Uuid { get; set; }
        public string? UrlBekendmaking { get; set; }
        public string? Toelichting { get; set; }
        public string? BeschikbaarRechtsmiddel { get; set; }
        public string? UrlReactieformulier { get; set; }
        public DateOnly? DatumBeginInzagetermijn { get; set; }
        public DateOnly? DatumEindeInzagetermijn { get; set; }
    }
}
