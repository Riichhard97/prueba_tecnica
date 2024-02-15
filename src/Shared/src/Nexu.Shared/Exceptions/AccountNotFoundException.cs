using System;

namespace Nexu.Shared.Exceptions
{
    /// <summary>
    /// Indicates that the specified account could not be found.
    /// </summary>
    [Serializable]
    public class AccountNotFoundException : Exception
    {
        public AccountNotFoundException(Guid id)
            : this($"Unable to find account {id}")
        {
        }

        public AccountNotFoundException()
        {
        }

        public AccountNotFoundException(string message) : base(message)
        {
        }

        public AccountNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AccountNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
