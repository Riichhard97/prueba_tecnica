using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Nexu.Shared.EntityFrameworkCore
{
    public class DbContextMigrator<TContext>
        where TContext : DbContext
    {
        protected TContext Context { get; }

        protected ILogger Logger { get; }

        public DbContextMigrator(TContext context, ILogger logger)
        {
            Context = context;
            Logger = logger;
        }

        public async Task Run(CancellationToken cancellationToken = default)
        {
            var pending = await Context.Database
                .GetPendingMigrationsAsync(cancellationToken)
                .ConfigureAwait(false);
            var migrations = pending as string[] ?? pending.ToArray();
            if (migrations.Length == 0)
            {
                Logger.LogInformation("No migrations to apply.");
                return;
            }

            Logger.LogInformation($"Preparing to apply {migrations.Length} migrations");
            await Context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);

            await OnMigrate(migrations, cancellationToken).ConfigureAwait(false);
        }

        protected virtual Task OnMigrate(string[] migrations, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
