using System;
using System.Threading.Tasks;

namespace Nexu.Shared.AspNetCore.Authentication
{
    public class BearerAuthenticationEvents
    {
        /// <summary>
        /// Invoked when a protocol message is first received.
        /// </summary>
        public Func<BearerMessageReceivedContext, Task> OnMessageReceived { get; set; } = _ => Task.CompletedTask;

        public virtual Task MessageReceived(BearerMessageReceivedContext context) => OnMessageReceived(context);
    }
}
