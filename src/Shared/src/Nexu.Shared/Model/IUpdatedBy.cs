using System;

namespace Nexu.Shared.Model
{
    public interface IUpdatedBy : IHaveDateUpdated
    {
        Guid? UpdatedById { get; }

        void SetUpdatedById(Guid id);
    }
}
