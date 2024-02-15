using System;

namespace Nexu.Shared.Infrastructure
{
    public interface ICurrentProjectAccessor
    {
        Guid Get();

        Guid? TryGet();
    }
}
