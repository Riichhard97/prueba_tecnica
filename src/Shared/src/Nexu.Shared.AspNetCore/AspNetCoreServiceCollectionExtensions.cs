using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nexu.Shared.AspNetCore;
using Nexu.Shared.AspNetCore.Filters;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AspNetCoreServiceCollectionExtensions
    {
        public static void AddCors(this IServiceCollection services, IConfiguration configuration, params string[] exposedHeaders)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var corsConfiguration = configuration.GetRequiredSection("CORS").Get<CorsConfiguration>();
            AddCors(services, corsConfiguration, exposedHeaders);
        }

        public static void AddCors(this IServiceCollection services, CorsConfiguration corsConfiguration, params string[] exposedHeaders)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (corsConfiguration is null)
            {
                throw new ArgumentNullException(nameof(corsConfiguration));
            }

            services.AddCors(options =>
            {
                options.DefaultPolicyName = CorsDefaults.PolicyName;
                options.AddPolicy("default", builder =>
                    builder.AllowAnyHeader()
                        //.SetIsOriginAllowed(host => host.StartsWith("http://localhost:"))
                        .WithExposedHeaders(exposedHeaders)
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithOrigins(corsConfiguration.Origins.Split(','))
                );
            });
        }

        public static IServiceCollection AddCustomExceptionHandler<T>(this IServiceCollection services)
            where T : class, ICustomExceptionHandler
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<ICustomExceptionHandler, T>());
            return services;
        }
    }
}
