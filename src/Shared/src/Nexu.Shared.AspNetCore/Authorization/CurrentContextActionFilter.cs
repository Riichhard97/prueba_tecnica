using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Nexu.Shared.AspNetCore.Authorization
{
    /// <summary>
    /// Resolves the userId and projectId in the current request, if provided.
    /// </summary>
    public class CurrentContextActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No-op
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var resolver = context.HttpContext.RequestServices.GetRequiredService<CurrentContextResolver>();
            resolver.Resolve(context.HttpContext);
        }
    }
}
