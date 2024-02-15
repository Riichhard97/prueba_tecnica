namespace Nexu.Shared.Infrastructure.Security
{
    public sealed class RsaConfiguration : RsaConfigurationBase
    {
        public string Password { get; set; }

        public string PrivateKey { get; set; }

        /// <summary>
        /// Lifetime of access token in seconds (defaults to 3600 seconds / 1 hour)
        /// </summary>
        public int AccessTokenLifetime { get; set; } = 60 * 60;
    }
}
