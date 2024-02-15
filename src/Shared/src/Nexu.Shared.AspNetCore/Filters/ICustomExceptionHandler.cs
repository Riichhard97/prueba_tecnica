using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Nexu.Shared.AspNetCore.Filters
{
    public interface ICustomExceptionHandler
    {
        Task OnExceptionAsync(ExceptionContext context);
    }
}
