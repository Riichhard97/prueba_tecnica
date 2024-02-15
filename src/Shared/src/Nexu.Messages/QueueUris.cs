using System;

namespace Nexu.Messages
{
    public static class QueueUris
    {
        public static readonly Uri Portal = new($"queue:{QueueNames.Portal}");
    }
}
