using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;

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
            Log.Error("Throwing HubException!");
            // This will propagate to the client by default (with no settings needed)
            throw new HubException("This error will be sent to the client!");
        }

        public Task ThrowNormalException()
        {
            Log.Error("Throwing ApplicationException!");
            // needs services.AddSignalR(x => x.EnableDetailedErrors = true) for the detail to be sent to the client
            throw new ApplicationException("An application exception");
        }

        public async IAsyncEnumerable<int> Counter(int count, int delay, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            for (var i = 0; i < count; i++)
            {
                // Check the cancellation token regularly so that the server will stop
                // producing items if the client disconnects.
                cancellationToken.ThrowIfCancellationRequested();

                yield return i;

                // Use the cancellationToken in other APIs that accept cancellation
                // tokens so the cancellation can flow down to them.
                await Task.Delay(delay, cancellationToken);
            }
        }
    }
}
