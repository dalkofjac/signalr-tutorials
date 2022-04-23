using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;


namespace SignalRSignalingServer.Hubs
{
    [AllowAnonymous]
    public class AuthHub : Hub
    {
        public Task<string> Authorize()
        {
            return Task.Run(() => { return TokenHelper.GenerateToken(); });
        }
    }
}
