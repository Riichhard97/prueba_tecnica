using System;
using System.Runtime.Serialization;

namespace Nexu.Shared.Exceptions
{
    [Serializable]
    public class AuthorizationException : Exception
    {
        public bool Forbidden { get; }

        public AuthorizationException(bool forbidden = false)
        {
            Forbidden = forbidden;
        }

        public AuthorizationException()
        {
        }

        public AuthorizationException(string message) : base(message)
        {
        }

        public AuthorizationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AuthorizationException(
          SerializationInfo info,
          StreamingContext context) : base(info, context)
        {
            Forbidden = (bool)info.GetValue(nameof(Forbidden), typeof(bool));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(Forbidden), Forbidden);

            base.GetObjectData(info, context);
        }
    }
}
