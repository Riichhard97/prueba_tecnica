using System;
using System.Runtime.Serialization;

namespace Nexu.Shared.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    /// Represents errors that occur due to entities that don't exist for a given key.
    /// </summary>
    [Serializable]
    public class EntityNotFoundException : Exception
    {
        public Type EntityType { get; }

        public object Key { get; }

        public EntityNotFoundException()
            : base(Properties.Resources.EntityNotFoundErrorMessage)
        {
        }

        public EntityNotFoundException(Type entityType, object key)
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public EntityNotFoundException(string message, Type entityType, object key) : base(message)
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public EntityNotFoundException(string message) : base(message)
        {
        }

        public EntityNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected EntityNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            EntityType = (Type)info.GetValue(nameof(EntityType), typeof(Type));
            Key = (string)info.GetValue(nameof(Key), typeof(string));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(EntityType), EntityType);

            info.AddValue(nameof(Key), nameof(Key));

            base.GetObjectData(info, context);
        }

        public static EntityNotFoundException For<TEntity>(object key)
        {
            return new EntityNotFoundException(typeof(TEntity), key);
        }

        public static EntityNotFoundException For<TEntity>(object key, string message)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return new EntityNotFoundException(message, typeof(TEntity), key);
        }
    }
}
