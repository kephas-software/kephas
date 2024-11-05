# Extensions

## Introduction
This package provides extensions to Microsoft infrastructure.

## Health checks

By default, the following health checks are provided:
* AppModules: monitors the application assembly versions.
* Memory: monitors the memory consumption.
* Process: monitors the process.

* Example of usage

```csharp
services.AddHealthCheckServices();

app.UseHealthCheckServices();
```

* Example of registering own health check

```csharp
[FailureStatus(HealthStatus.Unhealthy)]
[Tags("db")]
public class DatabaseHealthCheck : IHealthCheckService
{
    //...
}

// note that services implementing IHealthCheckService are automatically registered
services.AddHealthCheckServices();
```

> Note: register your custom health checks *before* invoking `services.AddHealthCheckServices()`, otherwise it will not be properly configured.

## Other resources

* [Kephas.Exceptions](https://www.nuget.org/packages/Kephas.Exceptions)
* [Kephas.DependencyInjection.Services](https://www.nuget.org/packages/Kephas.DependencyInjection.Services)
* [Kephas.Analyzers](https://www.nuget.org/packages/Kephas.Analyzers)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.

