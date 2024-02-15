using System;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Serilog;

namespace Nexu.Shared.AspNetCore
{
    /// <summary>
    /// Helper class to configure the host builder.
    /// </summary>
    public static class DefaultHostBuilder
    {
        /// <summary>
        /// Creates an <see cref="IHostBuilder"/> with the default configuration, using <typeparamref name="TStartup"/> as the
        /// startup type to be used by the host.
        /// </summary>
        /// <typeparam name="TStartup"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder Create<TStartup>(string[] args) where TStartup : class =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, builder) =>
                {
                    builder.ClearProviders();
                    var isDevelopment = context.HostingEnvironment.IsDevelopment();
                    builder.AddSimpleConsole(options =>
                    {
                        if (!isDevelopment)
                        {
                            options.ColorBehavior = LoggerColorBehavior.Disabled;
                        }
                        options.TimestampFormat = "[HH:mm:ss.fff] ";
                    });
                })
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TStartup>();
                });

        /// <summary>
        /// Configures the host to include Azure Key Vault configuration if present and configures the default logger.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="args">Command line arguments.</param>
        /// <returns></returns>
        public static void ConfigureHost(this IHostBuilder builder, string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environment == null)
            {
                environment = "Development";
            }

            var configurationBuilder = new ConfigurationBuilder()
                    .AddCommandLine(args)
                    .AddEnvironmentVariables()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

            configurationBuilder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
        }
    }
}
