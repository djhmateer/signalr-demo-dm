using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRDemoDM.Hubs
{
    public class FooHub : Hub
    {
        public async Task SendMessageToCaller(string user, string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessageFoo", user, message);
        }

        public Task ThrowException()
        {
            // This will propagate to the client by default (with no settings needed)
            throw new HubException("This error will be sent to the client!");
        }

        public Task ThrowNormalException()
        {
            throw new ApplicationException("An application exception");
        }
    }
}
