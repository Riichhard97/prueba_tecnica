using Microsoft.Extensions.DependencyInjection;
using Nexu.Shared.AspNetCore.Authorization;
using Nexu.Shared.Infrastructure;
using Serilog;

namespace Nexu.Shared.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers account and user accessors that automatically add the values to the current <see cref="IDiagnosticContext" />.
        /// </summary>
        /// <param name="services"></param>
        public static void AddDiagnosticContextAccessors(this IServiceCollection services)
        {
            services.AddScoped(x => new DiagnosticContextCurrentCustomerSetter(
                new CurrentProjectAccessor(), new CurrentUserAccessor(),
                new CurrentTokenAssesor(),x.GetRequiredService<IDiagnosticContext>()));

            services.AddScoped<ICurrentProjectAccessor>(x => x.GetRequiredService<DiagnosticContextCurrentCustomerSetter>());
            services.AddScoped<ICurrentProjectSetter>(x => x.GetRequiredService<DiagnosticContextCurrentCustomerSetter>());
            services.AddScoped<ICurrentUserAccessor>(x => x.GetRequiredService<DiagnosticContextCurrentCustomerSetter>());
            services.AddScoped<ICurrentUserSetter>(x => x.GetRequiredService<DiagnosticContextCurrentCustomerSetter>());
            services.AddScoped<ICurrentTokenAccessor>(x => x.GetRequiredService<DiagnosticContextCurrentCustomerSetter>());
            services.AddScoped<ICurrentTokenSetter>(x => x.GetRequiredService<DiagnosticContextCurrentCustomerSetter>());
            services.AddScoped<CurrentContextResolver>();
        }
    }
}
