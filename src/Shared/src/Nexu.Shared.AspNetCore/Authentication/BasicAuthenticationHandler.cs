using System;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Nexu.Shared.AspNetCore.Authentication
{
    public sealed class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return Task.FromResult(HandleAuthenticate());
        }

        private AuthenticateResult HandleAuthenticate()
        {
            string authorizationHeader = Request.Headers[HeaderNames.Authorization];
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return AuthenticateResult.NoResult();
            }

            if (!authorizationHeader.StartsWith(BasicAuthenticationDefaults.Scheme + ' ', StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.NoResult();
            }

            var encodedCredentials = authorizationHeader.Substring(BasicAuthenticationDefaults.Scheme.Length).Trim();

            if (string.IsNullOrEmpty(encodedCredentials))
            {
                const string noCredentialsMessage = "No credentials";
                Logger.LogInformation(noCredentialsMessage);
                return AuthenticateResult.Fail(noCredentialsMessage);
            }

            byte[] base64DecodedCredentials;
            try
            {
                base64DecodedCredentials = Convert.FromBase64String(encodedCredentials);
            }
            catch (FormatException)
            {
                const string failedToDecodeCredentials = "Cannot convert credentials from Base64.";
                Logger.LogInformation(failedToDecodeCredentials);
                return AuthenticateResult.Fail(failedToDecodeCredentials);
            }

            string decodedCredentials;
            try
            {
                decodedCredentials = Encoding.UTF8.GetString(base64DecodedCredentials);
            }
            catch (Exception ex)
            {
                const string failedToDecodeCredentials = "Cannot build credentials from decoded base64 value, exception {0} encountered.";
                var logMessage = string.Format(CultureInfo.InvariantCulture, failedToDecodeCredentials, ex.Message);
                Logger.LogInformation(logMessage);
                return AuthenticateResult.Fail(logMessage);
            }

            var delimiterIndex = decodedCredentials.IndexOf(":", StringComparison.OrdinalIgnoreCase);
            if (delimiterIndex == -1)
            {
                const string missingDelimiterMessage = "Invalid credentials, missing delimiter.";
                Logger.LogInformation(missingDelimiterMessage);
                return AuthenticateResult.Fail(missingDelimiterMessage);
            }

            var username = decodedCredentials.Substring(0, delimiterIndex);
            var password = decodedCredentials.Substring(delimiterIndex + 1);

            if (username == Options.ClientId && password == Options.Secret)
            {
                var claims = new[]
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        username,
                        ClaimValueTypes.String,
                        Options.ClaimsIssuer)
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }

            return AuthenticateResult.Fail("Invalid username and password");
        }
    }
}
