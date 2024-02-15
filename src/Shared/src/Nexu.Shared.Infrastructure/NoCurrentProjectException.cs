using System;

namespace Nexu.Shared.Infrastructure
{
    [Serializable]
    public class NoCurrentProjectException : Exception
    {
        public NoCurrentProjectException()
        {
        }

        public NoCurrentProjectException(string message) : base(message)
        {
        }

        public NoCurrentProjectException(string message, Exception inner) : base(message, inner)
        {
        }

        protected NoCurrentProjectException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
