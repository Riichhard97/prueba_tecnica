using System;

namespace Nexu.Shared.Infrastructure
{
    public interface ICurrentProjectSetter
    {
        void Set(Guid id);

        bool TrySet(Guid id);     
    }
}
