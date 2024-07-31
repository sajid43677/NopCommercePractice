using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Nop.Plugin.Widgets.NopStationEmployees;
public class TestMessageHub : Hub
{
    public async Task SendMessageAsync(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
