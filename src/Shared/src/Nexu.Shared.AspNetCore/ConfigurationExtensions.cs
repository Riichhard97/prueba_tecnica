using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Console;
using Serilog;
using Serilog.Extensions.Logging;

namespace Microsoft.Extensions.Logging
{
    public static class ConfigurationExtensions
    {
        public static ILoggerFactory CreateDefaultLoggerFactory(this IConfiguration configuration)
        {
            if (Log.Logger != null)
            {
                return new SerilogLoggerFactory(Log.Logger);
            }

            return LoggerFactory.Create(options =>
            {
                options.AddConfiguration(configuration.GetSection("Logging"));
                options.AddSimpleConsole(options =>
                {
                    options.ColorBehavior = LoggerColorBehavior.Disabled;
                });
            });
        }

        public static ILogger<T> CreateDefaultLogger<T>(this IConfiguration configuration)
        {
            return configuration.CreateDefaultLoggerFactory().CreateLogger<T>();
        }

        public static ILogger CreateDefaultLogger(this IConfiguration configuration, Type type)
        {
            return configuration.CreateDefaultLoggerFactory().CreateLogger(type);
        }
    }
}
