using System;

namespace Nexu.Shared.Infrastructure
{
    [Serializable]
    public class NoCurrentUserException : Exception
    {
        public NoCurrentUserException()
        {
        }

        public NoCurrentUserException(string message) : base(message)
        {
        }

        public NoCurrentUserException(string message, Exception inner) : base(message, inner)
        {
        }

        protected NoCurrentUserException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
