using MediatR;

namespace Nexu.Shared.Infrastructure.Commands
{
    public interface ICommand<out T> : IRequest<T>
    {
    }

    public interface ICommand : ICommand<Unit>, IRequest
    {
    }
}
