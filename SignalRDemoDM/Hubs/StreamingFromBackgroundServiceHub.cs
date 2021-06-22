using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace SignalRDemoDM.Hubs
{
    public class StreamingFromBackgroundServiceHub : Hub
    {
        // Async iterator methods avoid problems common with Channels, such as not returning the ChannelReader early enough
        // or exiting the method without completing the ChannelWriter<T>.
        // https://docs.microsoft.com/en-us/aspnet/core/signalr/streaming?view=aspnetcore-5.0#server-to-client-streaming



        // Javascript calls this method
        // which returns a ChannelReader<int>
        public ChannelReader<int> Counter(int count, int delay, CancellationToken cancellationToken)
        {
            // No limit to the size of this (so no backpressure)
            var channel = Channel.CreateUnbounded<int>();

            // We don't want to await WriteItemsAsync, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.

            // should this be in a BackgroundService? 
            _ = WriteItemsAsync(channel.Writer, count, delay, cancellationToken);

            return channel.Reader;
        }

        // Write an int to the channel
        // returns a Task only?
        private async Task WriteItemsAsync(ChannelWriter<int> writer, int count, int delay, CancellationToken cancellationToken)
        {
            Exception localException = null;
            try
            {
                for (var i = 0; i < count; i++)
                {
                    Log.Information($"Inside StreamingChannelHub {i}");

                    // write result back to channel
                    await writer.WriteAsync(i, cancellationToken);

                    // Use the cancellationToken in other APIs that accept cancellation
                    // tokens so the cancellation can flow down to them.
                    await Task.Delay(delay, cancellationToken);
                }

                throw new ApplicationException("ApplicationException - all stop!");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Inside WriteItemsAsync - Top level Exception caught");
                localException = ex;
            }
            finally
            {
                writer.Complete(localException);
            }
        }

    }
}
