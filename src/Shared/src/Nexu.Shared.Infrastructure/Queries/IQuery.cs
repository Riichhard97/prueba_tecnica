using MediatR;

namespace Nexu.Shared.Infrastructure.Queries
{
    public interface IQuery<out T> : IRequest<T>
    {
    }
}
