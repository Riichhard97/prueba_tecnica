using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Nexu.Shared.AspNetCore.Authentication
{
    public class AccountAuthenticationHandler : BearerAuthenticationHandler
    {
        protected override string HeaderName { get; } = "X-Account";

        public AccountAuthenticationHandler(IOptionsMonitor<BearerAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }
    }
}
