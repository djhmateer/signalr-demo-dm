using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using SignalRDemoDM.Hubs;

namespace SignalRDemoDM.BackgroundServices
{
    public class TestService : BackgroundService
    {
        private readonly IHubContext<StreamingFromBackgroundServiceHub> _hubContext;

        public TestService(IHubContext<StreamingFromBackgroundServiceHub> hubContext)
        {
            _hubContext = hubContext;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // todo try catch

            await Task.Delay(2000, stoppingToken);
            var message = "Started TestService Background Service";
            Log.Information(message);

            while (!stoppingToken.IsCancellationRequested)
            {
                Log.Information("ping");
                // https://github.com/dotnet/AspNetCore.Docs/issues/12427
                // how to get the clientId of signalr
                // or userId

                // this will broadcast to All clients!
                //await _hubContext.Clients.All.SendAsync("ReceiveMessage", "user1", "asdf");
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }

            Log.Information($"Stopped TestService Background Service");
        }
    }
}
