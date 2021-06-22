using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using SignalRDemoDM.BackgroundServices;
using SignalRDemoDM.Hubs;

namespace SignalRDemoDM
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            // Need EnableDetailedErrors for any exception to be propagated to the client (except for HubException)
            services.AddSignalR(x => x.EnableDetailedErrors = true);

            // Used by StreamFromBackgroundService.
            services.AddHostedService<TestService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // don't want request logging for static files so put it here in the pipeline
            //app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapHub<FooHub>("/fooHub");
                endpoints.MapHub<StreamingHub>("/streamingHub");
                endpoints.MapHub<CrawlHub>("/crawlHub");
                endpoints.MapHub<StreamingChannelHub>("/streamingChannelHub");
                endpoints.MapHub<StreamingFromBackgroundServiceHub>("/streamingFromBackgroundServiceHub");
            });
        }
    }
}
