using System;

namespace Nexu.Shared.Model
{
    public interface ICreatedBy : IHaveDateCreated
    {
        Guid? CreatedById { get; }

        void SetCreatedBy(Guid id);
    }

    public static class CreatedByExtensions
    {
        public static bool IsCreatedByNotSet(this ICreatedBy createdBy)
        {
            if (createdBy is null)
            {
                throw new ArgumentNullException(nameof(createdBy));
            }

            return createdBy.CreatedById == default;
        }

        public static void CheckCreatedByIdNotSet(this ICreatedBy createdBy)
        {
            if (createdBy is null)
            {
                throw new ArgumentNullException(nameof(createdBy));
            }

            if (createdBy.CreatedById != default)
            {
                throw new InvalidOperationException($"Cannot set {nameof(createdBy.CreatedById)}; it already has value {createdBy.CreatedById}.");
            }
        }
    }
}
