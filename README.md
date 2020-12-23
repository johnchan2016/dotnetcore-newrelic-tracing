## dotnetcore-newrelic-tracing

1. create license key (api key)
https://docs.newrelic.com/docs/telemetry-data-platform/ingest-manage-data/ingest-apis/use-event-api-report-custom-events

2a. install .net agent if not host IIS
open Command Prompt, type the following cmd:
msiexec.exe /i F:\NewRelicDotNetAgent_x64.msi /qb NR_LICENSE_KEY=eu01xx39d30618dde7994affd3ecd08dFFFFNRAL INSTALLLEVEL=50

2b. install .NET agent if host IIS using NuGet
https://docs.newrelic.com/docs/agents/net-agent/install-guides/install-net-agent-using-nuget


3. create insight insert key
  https://docs.newrelic.com/docs/telemetry-data-platform/ingest-manage-data/ingest-apis/use-event-api-report-custom-events

4. install lib
  <PackageReference Include="NewRelic.Agent" Version="8.36.0" />
  <PackageReference Include="NewRelic.Agent.Api" Version="8.36.0" />
  <PackageReference Include="NewRelic.LogEnrichers.Serilog" Version="1.0.1" />
  <PackageReference Include="NewRelic.OpenTelemetry" Version="1.0.0-rc1.9" />
  <PackageReference Include="Newtonsoft.Json" Version="12.0.3" />
  <PackageReference Include="OpenTelemetry" Version="1.0.0-rc1.1" />
  <PackageReference Include="OpenTelemetry.Api" Version="1.0.0-rc1.1" />
  <PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.0.0-rc1.1" />
  <PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.0.0-rc1.1" />
  <PackageReference Include="OpenTelemetry.Instrumentation.Http" Version="1.0.0-rc1.1" />
  <PackageReference Include="Serilog" Version="2.10.0" />
  <PackageReference Include="Serilog.AspNetCore" Version="3.4.0" />
  <PackageReference Include="Serilog.Settings.Configuration" Version="3.1.0" />
  <PackageReference Include="Serilog.Sinks.File" Version="4.1.0" />
  <PackageReference Include="Serilog.Sinks.NewRelic.Logs" Version="1.0.1" />

5. add following lines in Program.cs that export logs directly to New Relic
  Log.Logger = new LoggerConfiguration()
      .Enrich.WithNewRelicLogsInContext()
      //.ReadFrom.Configuration(configuration)
      .WriteTo.NewRelicLogs(
          // need to check the reason not to send log message to New Relic via API
          endpointUrl: configuration.GetValue<string>("NewRelic-Logging:EndpointUrl"), //endpoint depend on region chosen when account created
          applicationName: configuration.GetValue<string>("NewRelic-Logging:ServiceName"),
          insertKey: configuration.GetValue<string>("NewRelic-Logging:ApiKey"),
          restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information
        )
      .CreateLogger();

6. update newrelic.config
  <distributedTracing enabled="true" excludeNewrelicHeader="false"/>

6. install .net infrastructure agent if ship additional info from log file
https://docs.newrelic.com/docs/logs/enable-log-management-new-relic/enable-log-monitoring-new-relic/forward-your-logs-using-infrastructure-agent


## Generate logs for troubleshooting (.NET)
https://docs.newrelic.com/docs/agents/net-agent/troubleshooting/generate-logs-troubleshooting-net

Logs directory at %ALLUSERSPROFILE%\New Relic\.NET Agent\Logs
Agent logs: These file names begin with newrelic_agent_.
Profiler logs: These file names begin with NewRelic.Profiler.

7. add host name for resolving in DNS server 
c:\windows\system32\drivers\etc\hosts

8. grant apppool right for folder security to write log
https://blog.johnwu.cc/article/iis-run-asp-net-core.html
1. change .NET CLR Version to .net 4.0
2. right click folder -> security -> type "IIS AppPool\" + apppol name
