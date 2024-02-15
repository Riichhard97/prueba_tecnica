using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Nexu.Shared.AspNetCore.Authentication
{
    public abstract class BearerAuthenticationHandler : AuthenticationHandler<BearerAuthenticationOptions>
    {
        protected abstract string HeaderName { get; }

        private static readonly Action<ILogger, Exception> TokenValidationFailed = LoggerMessage.Define(LogLevel.Information,
            new EventId(1, nameof(LogTokenValidationFailed)),
            "Failed to validate the token.");

        private static readonly Action<ILogger, Exception> ProcessingMessageFailed = LoggerMessage.Define(LogLevel.Error,
            new EventId(2, nameof(LogProcessingMessageFailed)),
            "Successfully validated the token.");

        /// <summary>
        /// The handler calls methods on the events which give the application control at certain points where processing is occurring.
        /// If it is not provided a default instance is supplied which does nothing when the methods are called.
        /// </summary>
        protected new BearerAuthenticationEvents Events
        {
            get => (BearerAuthenticationEvents)base.Events;
            set => base.Events = value;
        }

        protected BearerAuthenticationHandler(IOptionsMonitor<BearerAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new BearerAuthenticationEvents());

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                // Give application opportunity to find from a different location, adjust, or reject token
                var messageReceivedContext = new BearerMessageReceivedContext(Context, Scheme, Options);

                // event can set the token
                await Events.MessageReceived(messageReceivedContext).ConfigureAwait(false);
                if (messageReceivedContext.Result != null)
                {
                    return messageReceivedContext.Result;
                }

                // If application retrieved token from somewhere else, use that.
                var token = messageReceivedContext.Token;
                if (string.IsNullOrEmpty(token))
                {
                    string authorization = Request.Headers[HeaderName];
                    // If no authorization header found, nothing to process further
                    if (string.IsNullOrEmpty(authorization))
                    {
                        return AuthenticateResult.NoResult();
                    }

                    token = authorization;
                    if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        token = authorization.Substring("Bearer ".Length).Trim();
                    }

                    // If no token found, no further work possible
                    if (string.IsNullOrEmpty(token))
                    {
                        return AuthenticateResult.NoResult();
                    }
                }

                var validationParameters = Options.TokenValidationParameters.Clone();
                List<Exception> validationFailures = null;
                foreach (var validator in Options.SecurityTokenValidators)
                {
                    if (validator.CanReadToken(token))
                    {
                        ClaimsPrincipal principal;
                        try
                        {
                            principal = validator.ValidateToken(token, validationParameters, out _);
                        }
                        catch (Exception ex)
                        {
                            LogTokenValidationFailed(Logger, ex);

                            if (validationFailures == null)
                            {
                                validationFailures = new List<Exception>(1);
                            }
                            validationFailures.Add(ex);
                            continue;
                        }

                        return HandleRequestResult.Success(new AuthenticationTicket(principal, Scheme.Name));
                    }
                }

                if (validationFailures != null)
                {
                    return AuthenticateResult.Fail((validationFailures.Count == 1) ? validationFailures[0] : new AggregateException(validationFailures));
                }

                return AuthenticateResult.Fail("No SecurityTokenValidator available for token: " + token ?? "[null]");
            }
            catch (Exception ex)
            {
                LogProcessingMessageFailed(Logger, ex);
                throw;
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            // TODO: Define header name and value to return
            Response.Headers.Add(HeaderName, "Unauthorized");
            return Task.CompletedTask;
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            // TODO: Define header name and value to return
            Response.Headers.Add(HeaderName, "Forbidden");
            return Task.CompletedTask;
        }

        private static void LogProcessingMessageFailed(ILogger logger, Exception exception)
        {
            ProcessingMessageFailed(logger, exception);
        }

        private static void LogTokenValidationFailed(ILogger logger, Exception exception)
        {
            TokenValidationFailed(logger, exception);
        }
    }
}
