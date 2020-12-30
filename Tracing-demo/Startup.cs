using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tracing_demo.Helper;

namespace Tracing_demo
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
            services.Configure<HttpUrl>(Configuration.GetSection("HttpUrl"));

            services.AddControllers();

            services.AddOpenTelemetryTracing((serviceProvider, tracerBuilder) =>
            {
                // Make the logger factory available to the dependency injection
                // container so that it may be injected into the OpenTelemetry Tracer.
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                // Adds the New Relic Exporter & enable distributed tracing in config
                tracerBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(this.Configuration.GetValue<string>("NewRelic:ServiceName")))
                    .AddNewRelicExporter(options =>
                    {
                        options.Endpoint = new Uri(this.Configuration.GetValue<string>("NewRelic:Endpoint"));
                        options.ApiKey = this.Configuration.GetValue<string>("NewRelic:ApiKey");
                    })
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<EnableResponseBufferMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
