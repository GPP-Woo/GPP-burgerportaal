namespace ODBP.Features.Onderwerpen
{
    public class Onderwerp
    {
        public Guid Uuid { get; set; }
        public List<string>? Publicaties { get; set; }
        public string? OfficieleTitel { get; set; }
        public string? Omschrijving { get; set; }
        public string? Afbeelding { get; set; }
        public string? Publicatiestatus { get; set; }
        public bool? Promoot { get; set; }
        public DateTime Registratiedatum { get; set; }
        public DateTime LaatstGewijzigdDatum { get; set; }
        
    }
}
