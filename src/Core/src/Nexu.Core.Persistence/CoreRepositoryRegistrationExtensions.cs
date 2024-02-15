using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nexu.Core.Persistence.Contracts;
using Nexu.Core.Persistence.Repositories;

namespace Nexu.Core.Persistence
{
    public static class CoreRepositoryRegistrationExtensions
    {
        public static void AddCoreRepository<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            services.AddScoped<IBrandRepository,BrandRepository<TContext>>();
        }
    }
}

