using System;

namespace Nexu.Shared.Infrastructure
{
    public class CurrentProjectAccessor : ICurrentProjectAccessor, ICurrentProjectSetter
    {
        private Guid? _id;

        protected bool IsSet => _id.HasValue;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public CurrentProjectAccessor()
        {
        }

        /// <summary>
        /// Initializes a new instance with a specific project id.
        /// </summary>
        /// <param name="id"></param>
        public CurrentProjectAccessor(Guid id)
        {
            _id = id;
        }

        public virtual Guid Get()
        {
            if (IsSet)
            {
                return _id.Value;
            }

            throw new NoCurrentProjectException();
        }

        public virtual Guid? TryGet()
        {
            return _id;
        }

        public virtual void Set(Guid id)
        {
            if (IsSet)
            {
                throw new InvalidOperationException($"Current project ID has already been set ({_id})");
            }

            _id = id;
        }

        public virtual bool TrySet(Guid id)
        {
            if (IsSet)
            {
                return false;
            }

            _id = id;
            return true;
        }
    }
}
