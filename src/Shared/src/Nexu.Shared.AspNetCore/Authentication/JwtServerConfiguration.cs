namespace Nexu.Shared.AspNetCore.Authentication
{
    public sealed class JwtServerConfiguration : JwtConfigurationBase
    {
        public string PrivateKey { get; set; }
    }
}
