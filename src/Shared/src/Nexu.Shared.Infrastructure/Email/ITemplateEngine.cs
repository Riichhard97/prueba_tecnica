using System;
using System.IO;
using System.Threading.Tasks;

namespace Nexu.Shared.Infrastructure.Email
{
    public interface ITemplateEngine
    {
        Task Render(string template, object model, TextWriter writer);
    }

    public static class TemplateEngineExtensions
    {
        public static async Task<string> Render(this ITemplateEngine engine, string template, object model)
        {
            if (engine is null)
            {
                throw new ArgumentNullException(nameof(engine));
            }

            if (template is null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            using var writer = new StringWriter();
            await engine.Render(template, model, writer).ConfigureAwait(false);
            return writer.ToString();
        }
    }
}
