using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nexu.Shared.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    /// Occurs when an object does not pass validation criteria.
    /// </summary>
    [Serializable]
    public class ObjectValidationException : Exception
    {
        private static readonly KeyValuePair<string, string>[] Empty = new KeyValuePair<string, string>[0];
        private readonly IEnumerable<KeyValuePair<string, string>> _errors;

        public IEnumerable<KeyValuePair<string, string>> Errors
        {
            get { return _errors ?? Empty; }
        }

        protected ObjectValidationException()
        {
        }

        public ObjectValidationException(IEnumerable<KeyValuePair<string, string>> errors)
        {
            _errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        public ObjectValidationException(string errorKey, string errorMessage)
            : base(errorMessage)
        {
            if (errorKey is null)
            {
                throw new ArgumentNullException(nameof(errorKey));
            }

            if (errorMessage is null)
            {
                throw new ArgumentNullException(nameof(errorMessage));
            }

            _errors = new[] { KeyValuePair.Create(errorKey, errorMessage) };
        }

        public ObjectValidationException(string message) : base(message)
        {
            _errors = new[] { KeyValuePair.Create(string.Empty, message) };
        }

        public ObjectValidationException(string message, Exception inner) : base(message, inner)
        {
            _errors = new[] { KeyValuePair.Create(string.Empty, message) };
        }

        protected ObjectValidationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            _errors = (IEnumerable<KeyValuePair<string, string>>)info.GetValue(nameof(Errors), typeof(IEnumerable<KeyValuePair<string, string>>));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(Errors), Errors);

            base.GetObjectData(info, context);
        }
    }
}
