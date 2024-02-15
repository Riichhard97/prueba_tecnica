using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Nexu.Shared.EntityFrameworkCore.Contracts;
using Nexu.Shared.Infrastructure.Persistence;

namespace Nexu.Shared.EntityFrameworkCore
{
    public static class RepositoryRegistrationExtensions
    {
        public static void AddRepository<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            services.AddScoped<IUnitOfWork,UnitOfWork<TContext>>();
            services.AddScoped(typeof(IUnitOfWorkRepositoryBase<>), typeof(UnitWorkRepositoryBase<>));
            services.AddTransient<IRepository, EfRepository<TContext>>();
            services.AddTransient<IReadOnlyRepository, ReadOnlyEfRepository<TContext>>();
        }

        public static void DisableRequiredNavigationWithQueryFilterInteractionWarning(this DbContextOptionsBuilder options)
        {
            options.ConfigureWarnings(builder =>
            {
                builder.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning);
            });
        }
    }
}
