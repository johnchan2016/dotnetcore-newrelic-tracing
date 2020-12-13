# dotnetcore-newrelic-tracing
1. install .net agent
msiexec.exe /i F:\NewRelicDotNetAgent_x64.msi /qb NR_LICENSE_KEY=eu01xx39d30618dde7994affd3ecd08dFFFFNRAL INSTALLLEVEL=50

2. install .net infrastructure agent
https://docs.newrelic.com/docs/logs/enable-log-management-new-relic/enable-log-monitoring-new-relic/forward-your-logs-using-infrastructure-agent

3. create license key

4. create insight insert key

5. install lib
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