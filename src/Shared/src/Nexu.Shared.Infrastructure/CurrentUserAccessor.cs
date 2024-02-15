using System;

namespace Nexu.Shared.Infrastructure
{
    public sealed class CurrentUserAccessor : ICurrentUserAccessor, ICurrentUserSetter
    {
        private Guid? _id;
        private string _name;
        private string _email;        

        public Guid Get()
        {
            if (_id.HasValue)
            {
                return _id.Value;
            }

            throw new NoCurrentUserException();
        }

        public Guid? TryGet()
        {
            return _id;
        }

        public void Set(Guid id)
        {
            if (_id.HasValue)
            {
                throw new InvalidOperationException($"Current user ID has already been set ({_id})");
            }

            _id = id;
        }

        public bool TrySet(Guid id)
        {
            if (_id.HasValue)
            {
                return false;
            }

            _id = id;
            return true;
        }

        public string GetName()
        {
            return _name;
        }

        public string GetEmail()
        {
            return _email;
        }

        public void SetName(string name)
        {
           _name = name;
        }

        public void SetEmail(string email)
        {
           _email = email;
        }
    }
}
