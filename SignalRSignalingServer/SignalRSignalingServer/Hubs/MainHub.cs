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

        public async Task SendMessage(string from, object message)
        {
            await Clients.All.SendAsync("message", from, message);
        }

        public async Task SendMessageToClient(string from, string to, object message)
        {
            var connection = ConnectedClients.FirstOrDefault(c => c.Value == to).Key;
            if (connection != null)
            {
                await Clients.Client(connection).SendAsync("message", from, message);
            }
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

        public async Task SendMessageToGroup(string from, string to, object message)
        {
            await Clients.OthersInGroup(to).SendAsync("message", from, message);
        }

        #endregion

        #region Overrides

        public override async Task OnConnectedAsync()
        {
            var client = Context.User?.Claims?.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier)?.Value;
            if (client != null) 
            {
                if (!ConnectedClients.ContainsKey(Context.ConnectionId))
                {
                    ConnectedClients.Add(Context.ConnectionId, client);
                }
                await Clients.AllExcept(Context.ConnectionId).SendAsync("connected", client);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (ConnectedClients.ContainsKey(Context.ConnectionId))
            {
                ConnectedClients.Remove(Context.ConnectionId);
            }
            var client = Context.User?.Claims?.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier)?.Value;
            if (client != null) 
            {
                await Clients.AllExcept(Context.ConnectionId).SendAsync("disconnected", client);
            }
            await base.OnDisconnectedAsync(exception);
        }

        #endregion
    }
}
