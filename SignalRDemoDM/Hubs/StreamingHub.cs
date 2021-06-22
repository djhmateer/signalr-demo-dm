using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace SignalRDemoDM.Hubs
{
    public class StreamingHub : Hub
    {
        public async IAsyncEnumerable<int> Counter(int count, int delay, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            for (var i = 0; i < count; i++)
            {
                // Check the cancellation token regularly so that the server will stop
                // producing items if the client disconnects.
                cancellationToken.ThrowIfCancellationRequested();
                Log.Information($"{i}");

                yield return i;

                // Use the cancellationToken in other APIs that accept cancellation
                // tokens so the cancellation can flow down to them.
                await Task.Delay(delay, cancellationToken);
            }
        }

       
    }
}
