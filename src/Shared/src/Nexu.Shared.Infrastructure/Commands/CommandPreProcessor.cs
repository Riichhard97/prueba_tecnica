using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;

namespace Nexu.Shared.Infrastructure.Commands
{
    public interface ICommandPreProcessor<TCommand> : IRequestPreProcessor<TCommand>
    {
    }

    public abstract class CommandPreProcessor<TCommand> : ICommandPreProcessor<TCommand>
    {
        public abstract Task Process(TCommand command, CancellationToken cancellationToken);
    }
}
