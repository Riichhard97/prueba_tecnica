using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;

namespace Nexu.Shared.Infrastructure.Commands
{
    public interface ICommandPostProcessor<TCommand, TResponse> : IRequestPostProcessor<TCommand, TResponse>
        where TCommand : ICommand
    {
    }

    public interface ICommandPostProcessor<TCommand> : IRequestPostProcessor<TCommand, Unit>
        where TCommand : ICommand
    {
    }

    public abstract class CommandPostProcessor<TCommand> : ICommandPostProcessor<TCommand>
        where TCommand : ICommand
    {
        public Task Process(TCommand command, Unit response, CancellationToken cancellationToken)
        {
            return Process(command, cancellationToken);
        }

        public abstract Task Process(TCommand command, CancellationToken cancellationToken);
    }
}
