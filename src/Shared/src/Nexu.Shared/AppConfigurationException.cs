using System;

namespace Nexu.Shared
{
    [Serializable]
    public class AppConfigurationException : Exception
    {
        public AppConfigurationException()
        {
        }

        public AppConfigurationException(string message) : base(message)
        {
        }

        public AppConfigurationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AppConfigurationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
