using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Nexu.Shared.Infrastructure.Commands
{
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand, Unit>
        where TCommand : ICommand
    {
        protected abstract Task Handle(TCommand command, CancellationToken cancellationToken);

        async Task<Unit> IRequestHandler<TCommand, Unit>.Handle(TCommand command, CancellationToken cancellationToken)
        {
            await Handle(command, cancellationToken).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
