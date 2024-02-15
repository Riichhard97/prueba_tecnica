using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Nexu.Shared.AspNetCore.Authentication
{
    /// <summary>
    /// Configures token-based authentication options to intercept and extract access tokens in query string parameters if present.
    /// This is useful for non-AJAX GET requests where Authorization headers cannot be set.
    /// </summary>
    public sealed class JwtQueryStringPostConfigureOptions : IPostConfigureOptions<JwtBearerOptions>, IPostConfigureOptions<BearerAuthenticationOptions>
    {
        public const string JwtParameterName = "access_token";
        public const string AccountParameterName = "account_token";
        public const string ImpersonateParameterName = "impersonate_token";

        public void PostConfigure(string name, JwtBearerOptions options)
        {
            if (options.Events is null)
            {
                options.Events = new JwtBearerEvents();
            }

            options.Events.OnMessageReceived = context =>
            {
                context.Token = ExtractTokenIfPresent(context, JwtParameterName);

                return Task.CompletedTask;
            };
        }

        public void PostConfigure(string name, BearerAuthenticationOptions options)
        {
            if (options.Events is null)
            {
                options.Events = new BearerAuthenticationEvents();
            }

            string parameterName;
            switch (name)
            {
                case AccountAuthenticationDefaults.DefaultAuthenticationScheme:
                    parameterName = AccountParameterName;
                    break;
                case ImpersonateAuthenticationDefaults.DefaultAuthenticationScheme:
                    parameterName = ImpersonateParameterName;
                    break;
                default:
                    return;
            }

            options.Events.OnMessageReceived = context =>
            {
                context.Token = ExtractTokenIfPresent(context, parameterName);

                return Task.CompletedTask;
            };
        }

        private static string ExtractTokenIfPresent<T>(ResultContext<T> context, string parameter)
            where T : AuthenticationSchemeOptions
        {
            if (!(HttpMethod.Get.Method.Equals(context.Request.Method) &&
                context.Request.Query.TryGetValue(parameter, out var values)))
            {
                return null;
            }

            if (values.Count > 1)
            {
                //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Fail(
                    $"Only one '{parameter}' query string parameter can be defined. " +
                    $"However, {values.Count:N0} were included in the request."
                );

                return null;
            }

            var token = values[0];
            if (string.IsNullOrWhiteSpace(token))
            {
                //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Fail(
                    $"The '{parameter}' query string parameter was defined, " +
                    "but a value to represent the token was not included."
                );

                return null;
            }

            return token;
        }
    }
}
