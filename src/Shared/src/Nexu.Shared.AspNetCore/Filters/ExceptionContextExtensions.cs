using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Nexu.Shared.AspNetCore.Filters
{
    public static class ExceptionContextExtensions
    {
        public static void BadRequest(this ExceptionContext context)
        {
            var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            var apiBehaviorOptions = options.Value;
            if (apiBehaviorOptions.InvalidModelStateResponseFactory != null)
            {
                context.Result = apiBehaviorOptions.InvalidModelStateResponseFactory(context);
            }
            else
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}
