using Microsoft.AspNetCore.Authentication;

namespace Nexu.Shared.AspNetCore.Authentication
{
    public sealed class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string ClientId { get; set; }

        public string Secret { get; set; }
    }
}
