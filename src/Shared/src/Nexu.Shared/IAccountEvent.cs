using System;

namespace Nexu.Shared
{
    public interface IAccountEvent
    {
        Guid AccountId { get; }
    }
}
