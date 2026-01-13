namespace ODBP.Authentication
{
    public class AuthOptions
    {
        public string Authority { get; set; } = "";
        public string ClientId { get; set; } = "";
        public string ClientSecret { get; set; } = "";
        public string AdminRole { get; set; } = "";
        public string? RoleClaimType { get; set; }
        public string? NameClaimType { get; set; }
        public string? IdClaimType { get; set; }
    }
}
