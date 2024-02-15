namespace Nexu.Shared.AspNetCore.Authentication
{
    public abstract class JwtConfigurationBase
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }
    }
}
