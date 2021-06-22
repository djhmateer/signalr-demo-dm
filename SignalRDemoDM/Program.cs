using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;

namespace SignalRDemoDM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                //.MinimumLevel.Information() // this is the default
                // Suppress framework log noise eg routing and handling
                // so we'll see warnings and errors from the framework
                // good setting 
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)

                // useful serilog
                //.MinimumLevel.Override("Microsoft.AspNetCore.SignalR", LogEventLevel.Debug)
                //.MinimumLevel.Override("Microsoft.AspNetCore.Http.Connections", LogEventLevel.Debug)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(@"logs/warning.txt", restrictedToMinimumLevel: LogEventLevel.Warning, rollingInterval: RollingInterval.Day)
                // careful - lots of data here
                .WriteTo.File(@"logs/info.txt", restrictedToMinimumLevel: LogEventLevel.Information, rollingInterval: RollingInterval.Day)
                .CreateLogger();
            try
            {
                Log.Information("");
                Log.Information("Starting up SignalRDemo");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
