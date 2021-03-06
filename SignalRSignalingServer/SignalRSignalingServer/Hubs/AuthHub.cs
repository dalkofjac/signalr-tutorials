using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;


namespace SignalRSignalingServer.Hubs
{
    [AllowAnonymous]
    public class AuthHub : Hub
    {
        public async Task<string> Authorize(string username)
        {
            return await Task.Run(() => { return TokenHelper.GenerateToken(username); });
        }
    }
}
