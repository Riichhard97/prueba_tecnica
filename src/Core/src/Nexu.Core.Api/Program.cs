using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nexu.Core.Persistence;
using Nexu.Shared.AspNetCore;

namespace Nexu.Core.Api
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = CreateHostBuilder(args);
            await HostRunner.Run(builder, Migrate, args);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            DefaultHostBuilder.Create<Startup>(args);

        private static async Task Migrate(IServiceProvider serviceProvider)
        {
            // Run database migrations
            var migrator = serviceProvider.GetRequiredService<CoreDbContextMigrator>();
            await migrator.Run();

            // Seed database after the migrations are done
            var dbInitializer = serviceProvider.GetService<CoreDbInitializer>();
            await dbInitializer.Run();
        }
    }
}
