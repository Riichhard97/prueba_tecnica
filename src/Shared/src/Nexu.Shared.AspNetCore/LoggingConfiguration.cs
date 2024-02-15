using System.Diagnostics;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Nexu.Shared.AspNetCore
{
    public static class LoggingConfiguration
    {
        public static void Configure(IConfiguration config)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.With(new DemystifiedStackTraceEnricher());

            Log.Logger = loggerConfiguration
                .CreateLogger();
        }
    }

    public sealed class DemystifiedStackTraceEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.Exception?.Demystify();
        }
    }
}
