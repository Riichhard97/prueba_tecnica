using System;

namespace Nexu.Shared.Model
{
    public interface IDeletedBy : ISoftDelete
    {
        Guid? DeletedById { get; }

        void SetDeletedById(Guid id);
    }
}
