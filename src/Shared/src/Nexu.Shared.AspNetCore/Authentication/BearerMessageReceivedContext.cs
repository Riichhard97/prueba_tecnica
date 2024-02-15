using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Nexu.Shared.AspNetCore.Authentication
{
    public class BearerMessageReceivedContext : ResultContext<BearerAuthenticationOptions>
    {
        public BearerMessageReceivedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            BearerAuthenticationOptions options)
            : base(context, scheme, options) { }

        /// <summary>
        /// Bearer Token. This will give the application an opportunity to retrieve a token from an alternative location.
        /// </summary>
        public string Token { get; set; }
    }
}
