using System;

namespace Nexu.Shared.Infrastructure
{
    public interface ICurrentUserSetter
    {
        void Set(Guid id);
        bool TrySet(Guid id);
        void SetName(string name);        
        void SetEmail(string email);        
    }
}
