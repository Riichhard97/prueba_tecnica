using System;
using Microsoft.AspNetCore.Http;
using Serilog.AspNetCore;
using Serilog.Events;

namespace Nexu.Shared.AspNetCore
{
    public static class RequestLoggingOptionsExtensions
    {
        public static void IgnoreHealthchecks(this RequestLoggingOptions options)
        {
            var getLevel = options.GetLevel;
            options.GetLevel = (context, _, exception) =>
            {
                if (IsHealthCheckEndpoint(context))
                {
                    return LogEventLevel.Verbose;
                }
                return getLevel(context, _, exception);
            };
        }

        private static bool IsHealthCheckEndpoint(HttpContext ctx)
        {
            var endpoint = ctx.GetEndpoint();
            if (endpoint is not null)
            {
                return string.Equals(
                    endpoint.DisplayName,
                    "Health checks",
                    StringComparison.Ordinal);
            }
            // No endpoint, so not a health check endpoint
            return false;
        }
    }
}
