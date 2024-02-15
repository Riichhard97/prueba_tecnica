using System;
using MediatR.Pipeline;

namespace Nexu.Shared.Infrastructure.Commands
{
    public interface ICommandExceptionAction<in TCommand, in TException> : IRequestExceptionAction<TCommand, TException>
        where TException : Exception
    {
    }

    public interface ICommandExceptionAction<in TCommand> : ICommandExceptionAction<TCommand, Exception>
    {
    }
}
