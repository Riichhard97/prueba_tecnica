using System;

namespace Nexu.Shared.Infrastructure
{
    public interface ICurrentUserAccessor
    {
        Guid Get();
        string GetName();
        string GetEmail();
        Guid? TryGet();
    }
}
