using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Nexu.Shared.AspNetCore
{
    /// <summary>
    /// Based on https://blog.stephencleary.com/2016/11/streaming-zip-on-aspnet-core.html
    /// </summary>
    public sealed class DownloadStreamResult : FileResult
    {
        public Func<Stream, CancellationToken, Task> Callback { get; }

        public long? Length { get; set; }

        public DownloadStreamResult(string contentType, Func<Stream, CancellationToken, Task> callback) : base(contentType)
        {
            Callback = callback;
        }

        /// <inheritdoc />
        public override Task ExecuteResultAsync(ActionContext context)
        {
            var executor = new DownloadStreamResultExecutor(context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>());
            return executor.ExecuteAsync(context, this);
        }

        private sealed class DownloadStreamResultExecutor : FileResultExecutorBase, IActionResultExecutor<DownloadStreamResult>
        {
            public DownloadStreamResultExecutor(ILoggerFactory loggerFactory) : base(CreateLogger<DownloadStreamResultExecutor>(loggerFactory))
            {
            }

            public Task ExecuteAsync(ActionContext context, DownloadStreamResult result)
            {
                var (range, rangeLength, serveBody) = SetHeadersAndLog(
                    context,
                    result,
                    fileLength: result.Length,
                    result.EnableRangeProcessing,
                    result.LastModified,
                    result.EntityTag);

                if (!serveBody)
                {
                    return Task.CompletedTask;
                }

                var feature = context.HttpContext.Features.Get<IHttpBodyControlFeature>();
                if (feature != null)
                {
                    feature.AllowSynchronousIO = true;
                }
                return result.Callback(context.HttpContext.Response.Body, context.HttpContext.RequestAborted);
            }
        }
    }
}
