using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PA.Genety.Shared.SignalR.Hubs
{
    // TODO: Add the AuthConstants from Shared.AspN
    [Authorize(Policy = "WorkspaceAdministratorOrUser")]
    public class MessageBrokerHub : Hub
    {
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }

        public async Task SendOtherNotification(string message)
        {
            await Clients.Others.SendAsync("ReceiveNotification", message);
        }

        public async Task SendWarning(string message)
        {
            await Clients.All.SendAsync("ReceiveWarning", message);
        }

        public async Task BlockField(string idProject, string idModule, string idField, string value)
        {
            await Clients.Others.SendAsync("BlockField", idProject, idModule, idField, value);
        }

        public async Task UnblockField(string idProject, string idModule, string idField)
        {
            await Clients.Others.SendAsync("UnblockField", idProject, idModule, idField);
        }
    }
}
