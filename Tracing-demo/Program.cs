using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewRelic.LogEnrichers.Serilog;
using Serilog;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Tracing_demo
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //從 appsettings.json 讀取設定資料
            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

            //使用從 appsettings.json 讀取到的內容來設定 logger
            Log.Logger = new LoggerConfiguration()
                    .Enrich.WithNewRelicLogsInContext()
                    //.ReadFrom.Configuration(configuration)
                    .WriteTo.NewRelicLogs(
                        // need to check the reason not to send log message to New Relic via API
                        applicationName: configuration.GetValue<string>("NewRelic-Logging:ServiceName"),
                        licenseKey: configuration.GetValue<string>("NewRelic-Logging:LicenseKey"),
                        insertKey: configuration.GetValue<string>("NewRelic-Logging:ApiKey"),
                        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information
                     )
                    .CreateLogger();

            CreateHostBuilder(args).Build().Run();
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
