using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Net.Http.Headers;
using Nexu.Shared.Infrastructure;

namespace Nexu.Shared.AspNetCore
{
    public static class HttpProxyExtensions
    {
        public static IHttpClientBuilder AddHttpProxy<TClient, TImplementation>(this IServiceCollection services, IConfiguration configuration)
            where TClient : class, IHttpProxy
            where TImplementation : class, TClient
        {
            var uri = configuration.GetRequiredValue<Uri>("Url");

            if (!uri.IsAbsoluteUri)
            {
                throw new InvalidOperationException($"Expected absolute URL for proxy service {typeof(TClient)}, but got '{uri}'");
            }

            var account = configuration.GetRequiredValue("ClientID");
            var password = configuration.GetRequiredValue("Secret");

            return services.AddHttpClient<TClient, TImplementation>(client =>
            {
                client.BaseAddress = uri;
                var headerValue = GetBasicAuthenticationHeaderValue(account, password);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", headerValue);
            })
#if DEBUG
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
                    ServerCertificateCustomValidationCallback = (sender, certificate, chain, errors) => true
#pragma warning restore S4830 // Server certificates should be verified during SSL/TLS connections
                };
            })
#endif
            ;
        }

        public static IServiceCollection AddDefaultHeaderPropagation(this IServiceCollection services)
        {
            return services.AddHeaderPropagation(options =>
            {
                options.Headers.Add(HeaderNames.AcceptLanguage);
            });
        }

        public static IHealthChecksBuilder AddHttpProxyHealthCheck<T>(this IHealthChecksBuilder services, HealthStatus? failureStatus = null, IEnumerable<string> tags = null, TimeSpan? timeout = null)
            where T : IHttpProxy
        {
            return services.AddCheck<HttpProxyHealthCheck<T>>(typeof(T).Name, failureStatus, tags, timeout);
        }

        private static string GetBasicAuthenticationHeaderValue(string account, string password)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes($"{account}:{password}"));
        }

        private sealed class HttpProxyHealthCheck<T> : IHealthCheck
            where T : IHttpProxy
        {
            private readonly T _proxy;

            public HttpProxyHealthCheck(T proxy)
            {
                _proxy = proxy;
            }

            public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                try
                {
                    var healthy = await _proxy.HealthCheck(cancellationToken).ConfigureAwait(false);
                    return healthy ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
                }
                catch (Exception e)
                {
                    return HealthCheckResult.Unhealthy(e.Message);
                }
            }
        }
    }
}
