using Microsoft.AspNetCore.SignalR;
using StudentNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentNetwork
{
    public class ChatHub : Hub
    {
        public async Task Send(Message message)
        {
            await Clients.All.SendAsync("Send", message).ConfigureAwait(true);
        }
    }
}
