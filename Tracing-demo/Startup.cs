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
                    .AddAspNetCoreInstrumentation((options) => options.Enrich
                        = async (activity, eventName, rawObject) =>
                        {
                            if (eventName.Equals("OnStartActivity") && rawObject is HttpRequest httpRequest)
                            {
                                if (httpRequest.Method != HttpMethods.Get)
                                {
                                    string body = await HttpHelper.GetRequestBodyStringAsync(httpRequest);
                                    if (!string.IsNullOrEmpty(body))
                                    {
                                        var dto = JsonConvert.DeserializeObject(body);
                                        var tags = new ActivityTagsCollection { new KeyValuePair<string, object?>("values", dto) };

                                        activity.AddEvent(new ActivityEvent("HttpRequestBody"));
                                    }
                                }

                                activity.SetTag("requestProtocol", httpRequest.Protocol);
                            }
                            //cannot get response result, so skip this
                            /*                            else if (eventName.Equals("OnStopActivity") && rawObject is HttpResponse httpResponse)
                                                        {
                                                            //&& httpResponse.StatusCode >= StatusCodes.Status400BadRequest
                                                            if (httpResponse.HttpContext.Request.Method != HttpMethods.Get )
                                                            {
                                                                string body = await HttpHelper.GetResponseBodyStringAsync(httpResponse);
                                                                var tags = new ActivityTagsCollection { new KeyValuePair<string, object?>("values", body) };
                                                                activity.AddEvent(new ActivityEvent("HttpResponseBody", DateTime.Now, tags));
                                                            }

                                                            activity.SetTag("responseLength", httpResponse.ContentLength);
                                                        }*/
                        }
                    )
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
