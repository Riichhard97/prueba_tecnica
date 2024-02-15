using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PA.Genety.Shared.SignalR.Hubs
{
    // TODO: Add the AuthConstants from Shared.AspNetCore
    [Authorize(Policy = "WorkspaceAdministratorOrUser")]
    public class ChatHub : Hub
    {
        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task SendMessage(string user, string targetConnectionId, string message)
        {
            await Clients.User(targetConnectionId).SendAsync("ReceiveMessage", user, targetConnectionId, message);
        }

        public async Task SendRoomMessage(string roomName, string user, string message)
        {
            await Clients.Group(roomName).SendAsync("ReceiveRoomMessage", user, message);
        }
    }
}
