using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chatapp
{
    public class chatHub : Hub
    {
        public void send(string name, string message, string groupsend)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<chatHub>();
            context.Clients.All.addNewMessageToPage(name, message, groupsend);
        }
    }
}