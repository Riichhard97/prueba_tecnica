using System;
using Nexu.Shared.Model;

namespace Nexu.Shared.Common
{
    public class BaseDomain : Entity,
        IHaveDateCreated,
        IHaveDateUpdated,
        IHaveDateDeleted,
        ICreatedBy,
        IUpdatedBy,
        IDeletedBy
    {
        public DateTime? DateCreated { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? DateDeleted { get; set; }
        public Guid? DeletedById { get; set; }
        public bool IsDeleted { get; set; }

        public void SetCreatedBy(Guid id)
        {
            CreatedById = id;
        }
        public void SetUpdatedById(Guid id)
        {
            UpdatedById = id;
        }

        public void SetDeletedById(Guid id)
        {
            DeletedById = id;
        }

        public void SetDeleted()
        {
            IsDeleted = DeletedById != Guid.Empty;
        }

        public bool IsNull()
        {
            return Id.Equals(Guid.Empty);
        }
    }
}
