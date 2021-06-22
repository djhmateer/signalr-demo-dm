using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace SignalRDemoDM.BackgroundServices
{
    public class TestService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // todo try catch

            await Task.Delay(2000, stoppingToken);
            Log.Information("Started TestService Background Service");

            while (!stoppingToken.IsCancellationRequested)
            {
                Log.Information("ping");
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }

            Log.Information($"Stopped TestService Background Service");
        }
    }
}
