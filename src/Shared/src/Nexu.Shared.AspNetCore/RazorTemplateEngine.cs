using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Nexu.Shared.Infrastructure.Email
{
    public sealed class RazorTemplateEngine : ITemplateEngine
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public RazorTemplateEngine(IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task Render(string template, object model, TextWriter writer)
        {
            if (template is null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            var actionContext = CreateActionContext();
            var viewResult = _razorViewEngine.FindView(actionContext, template, false);

            if (viewResult.View == null)
            {
                throw new InvalidOperationException(
                    $"{template} does not match any available view. " +
                    $"Searched locations: {string.Join(", ", viewResult.SearchedLocations)}");
            }

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewDictionary,
                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                writer,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext).ConfigureAwait(false);
        }

        private ActionContext CreateActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };

            var routeData = new RouteData();
            routeData.Values.Add("controller", "EmailTemplates");

            var actionDescriptor = new ActionDescriptor();
            return new ActionContext(httpContext, routeData, actionDescriptor);
        }
    }
}
