using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace SignalRSignalingServer.Hubs
{
    [Authorize]
    public class MainHub : Hub
    {
        #region General

        public async Task SendMessage(object message)
        {
            await Clients.All.SendAsync("Message", message);
        }

        public async Task SendMessageToClient(string client, object message)
        {
            await Clients.Client(client).SendAsync("Message", message);
        }

        #endregion

        #region Groups

        public async Task JoinGroup(string group)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
        }

        public async Task LeaveGroup(string group)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        }

        public async Task SendMessageToGroup(string group, object message)
        {
            await Clients.OthersInGroup(group).SendAsync("Message", message);
        }

        #endregion

        #region Overrides

        public override async Task OnConnectedAsync()
        {
            await Clients.AllExcept(Context.ConnectionId).SendAsync("ClientConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.AllExcept(Context.ConnectionId).SendAsync("ClientDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        #endregion
    }
}
