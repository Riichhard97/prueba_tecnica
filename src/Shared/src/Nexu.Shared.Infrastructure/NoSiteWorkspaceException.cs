using System;

namespace Nexu.Shared.Infrastructure
{
    [Serializable]
    public class NoSiteWorkspaceException : Exception
    {
        public NoSiteWorkspaceException()
        {
        }

        public NoSiteWorkspaceException(string message) : base(message)
        {
        }

        public NoSiteWorkspaceException(string message, Exception inner) : base(message, inner)
        {
        }

        protected NoSiteWorkspaceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
