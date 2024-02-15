using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Nexu.Shared.RemoteServices.EventHandlers;

namespace Nexu.Shared.RemoteServices.Filters
{
    public class AccountActiveHandler : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context,
        ActionExecutionDelegate next)
        {                               
            context.HttpContext.Request.Headers.TryGetValue("Authorization", out var jwtToken);      
            if(string.IsNullOrEmpty(jwtToken)) await next();
                       
            var userEventHandler = context.HttpContext.RequestServices.GetService<IUserEventHandler>();       
            if (!await userEventHandler.IsActive())
            {            
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "401 Unauthorized Error"
                };
                return;
             }
             await next();
        }
    }
}
