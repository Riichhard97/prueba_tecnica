using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Nexu.Shared.AspNetCore
{
    public static class HostRunner
    {
        public static async Task Run(IHostBuilder builder, string[] args)
        {
            builder.ConfigureHost(args);
            var host = builder.Build();

            try
            {
                await host.RunAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The Application failed to start.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static async Task Run(IHostBuilder builder, Func<IServiceProvider, Task> migrate, string[] args)
        {
            builder.ConfigureHost(args);
            var host = builder.Build();

            await Run(host, migrate).ConfigureAwait(false);
        }

        public static async Task Run(IHost host, Func<IServiceProvider, Task> migrate)
        {
            try
            {
                var environment = host.Services.GetRequiredService<IHostEnvironment>();
                var isDevelopment = environment.IsDevelopment();
                if (migrate != null && (isDevelopment || ShouldRunMigration()))
                {
                    using var scope = host.Services.CreateScope();
                    await migrate(scope.ServiceProvider).ConfigureAwait(false);

                    // Axel: This was causing environments other than Development to fail at startup.
                    //// In development mode, we fall through and run the host
                    //if (!isDevelopment)
                    //{
                    //    return;
                    //}
                }

                await host.RunAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The Application failed to start.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static bool ShouldRunMigration()
        {
            return bool.TrueString.Equals(Environment.GetEnvironmentVariable("RUN_MIGRATIONS"), StringComparison.OrdinalIgnoreCase);
        }
    }
}
