using System;
using MediatR;
using MediatR.Pipeline;

namespace Nexu.Shared.Infrastructure.Commands
{
    public interface ICommandExceptionHandler<in TCommand, TResponse, TException> : IRequestExceptionHandler<TCommand, TResponse, TException>
        where TException : Exception
    {
    }

    public interface ICommandExceptionHandler<in TCommand, TException> : ICommandExceptionHandler<TCommand, Unit, TException>
        where TException : Exception
    {
    }

    public interface ICommandExceptionHandler<in TCommand> : ICommandExceptionHandler<TCommand, Unit, Exception>
        where TCommand : ICommand<Unit>
    {
    }
}
