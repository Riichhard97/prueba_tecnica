using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexu.Shared.AspNetCore.Exceptions;
using Nexu.Shared.Exceptions;

namespace Nexu.Shared.AspNetCore.Filters
{
    public class ExceptionHandlerFilter : IAsyncExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            await OnCustomException(context).ConfigureAwait(false);

            if (context.Result != null)
            {
                // Custom exception handled by subclass
                return;
            }

            var exception = context.Exception;

            if (exception is EntityNotFoundException)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status404NotFound);
                return;
            }

            if (exception is BadRequestException || exception is ValidationException || exception is NotFoundException)
            {
                return;
            }

            if (exception is ObjectValidationException objectValidationException)
            {
                BadRequest(context, objectValidationException);
                return;
            }

            if (exception is BusinessLogicException)
            {
                context.ModelState.AddModelError("", exception.Message);
                context.BadRequest();
                return;
            }

            if (exception is AuthorizationException authException)
            {
                if (authException.Forbidden)
                {
                    context.Result = new ForbidResult();
                }
                else
                {
                    context.Result = new UnauthorizedResult();
                }
                return;
            }

            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<ExceptionHandlerFilter>>();

            logger.LogError(exception,
                $"Unhandled exception: {exception.Message}");

            InternalServerError(context, exception);
        }

        protected virtual async Task OnCustomException(ExceptionContext context)
        {
            var handlers = context.HttpContext.RequestServices
                .GetRequiredService<IEnumerable<ICustomExceptionHandler>>();

            foreach (var handler in handlers)
            {
                await handler.OnExceptionAsync(context).ConfigureAwait(false);
                if (context.ExceptionHandled || context.Result != null)
                {
                    break;
                }
            }
        }

        private static void InternalServerError(ExceptionContext context, Exception exception)
        {
            var environment = context.HttpContext.RequestServices
                .GetRequiredService<IWebHostEnvironment>();
            if (environment.IsDevelopment())
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Result = new JsonResult(new
                {
                    error = new[] { exception.Message },
                    stackTrace = exception.StackTrace
                });
                return;
            }

            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        private static void BadRequest(ExceptionContext context, ObjectValidationException exception)
        {
            foreach (var kvp in exception.Errors)
            {
                context.ModelState.AddModelError(kvp.Key, kvp.Value);
            }

            if (context.ModelState.Count == 0)
            {
                context.ModelState.AddModelError("", exception.Message);
            }

            context.BadRequest();
        }
    }
}
