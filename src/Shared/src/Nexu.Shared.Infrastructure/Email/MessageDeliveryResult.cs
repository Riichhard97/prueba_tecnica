namespace Nexu.Shared.Infrastructure.Email
{
    public class MessageDeliveryResult
    {
        public bool Succeeded { get; }

        public string MessageId { get; }

        public string ErrorMessage { get; }

        private MessageDeliveryResult(bool succeeded, string messageId, string errorMessage)
        {
            Succeeded = succeeded;
            MessageId = messageId;
            ErrorMessage = errorMessage;
        }

        public static MessageDeliveryResult Success(string messageId)
        {
            return new MessageDeliveryResult(true, messageId, null);
        }

        public static MessageDeliveryResult Error(string message)
        {
            return new MessageDeliveryResult(false, null, message);
        }
    }
}
