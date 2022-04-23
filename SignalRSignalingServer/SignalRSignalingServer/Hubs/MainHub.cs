using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SignalRSignalingServer.Hubs
{
    [Authorize]
    public class MainHub : Hub
    {
        public static Dictionary<string, string> ConnectedClients = new Dictionary<string, string>();

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
            var clientUsername = Context.User?.Claims?.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier)?.Value;
            if (clientUsername != null) 
            {
                if (!ConnectedClients.ContainsKey(Context.ConnectionId))
                {
                    ConnectedClients.Add(Context.ConnectionId, clientUsername);
                }
                await Clients.AllExcept(Context.ConnectionId).SendAsync("ClientConnected", clientUsername);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (ConnectedClients.ContainsKey(Context.ConnectionId))
            {
                ConnectedClients.Remove(Context.ConnectionId);
            }
            var clientUsername = Context.User?.Claims?.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier)?.Value;
            if (clientUsername != null) 
            {
                await Clients.AllExcept(Context.ConnectionId).SendAsync("ClientDisconnected", Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        #endregion
    }
}
