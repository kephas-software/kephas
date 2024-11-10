# Extensions - Health Check Diagnostics

## Introduction
This package provides extensions to Microsoft infrastructure in the Health Check area.

## Health checks

By default, the following health checks are provided:
* AppModules: monitors the application assembly versions.
* Memory: monitors the memory consumption.
* Process: monitors the process.

* Example of usage

```csharp
// use the /health path
app.UseHealthChecks("/health");

// for a more detailed report, add also this call:
// note that the path is optional, and can be configured using `HealthCheckSettings`.
app.UseHealthChecksReport("/health-report");
```

* Example of registering own health check

```csharp
[FailureStatus(HealthStatus.Unhealthy)]
[Tags("db")]
public class DatabaseHealthCheck : IHealthCheck
{
    //...
}

// note that services implementing IHealthCheck are automatically registered
```

## Other resources

* [Kephas.Exceptions](https://www.nuget.org/packages/Kephas.Exceptions)
* [Kephas.DependencyInjection.Services](https://www.nuget.org/packages/Kephas.DependencyInjection.Services)
* [Kephas.Analyzers](https://www.nuget.org/packages/Kephas.Analyzers)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.

