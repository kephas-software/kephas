# Enhancements for `Microsoft.Extensions.DependencyInjection`

## Introduction
Provides integration of services collected through Kephas with the `Microsoft.Extensions.DependencyInjection` infrastructure
and adds support for service metadata.

## Usage

#### Add application services and build a service provider 

```csharp
var services = new ServiceCollection();
services.AddApplicationServices(configuration);
var serviceProvider = services.BuildServiceProvider();
```

#### Add application services while configuring the application runtime

An example for tests, where the entry assembly and the assembly name pattern are set explicitly:

```csharp
var appRuntimeSettings = new AppRuntimeSettings
{
    // the testing infrastructure does not have an entry assembly, that is why we need to set it explicitly.
    EntryAssembly = typeof(TTestClass).Assembly,
    AppAssemblyNamePattern = AppRuntimeSettings.TestsIncludedAppAssemblyNamePattern,
};

var serviceProvider = new ServiceCollection()
    .AddApplicationServices(Configuration, new AppRuntime(appRuntimeSettings))
    .BuildServiceProvider();
```

## Integration testing

When dealing with integration tests, the `IntegrationTestBase` might prove handy, as well as the `Test` class, both defined in
the [Kephas.Testing.Xunit](https://www.nuget.org/packages/Kephas.Testing.Xunit) package.

Hints for test configuration:
* `Constructor`: Add additional services in the test class constructor, *before* the service provider is built.
* `InitializeAsync`: Use the service provider to retrieve services and do any initialization *after* the service provider is built.
* `DisposeAsync`: Do any cleanup required.

> Notes: avoid using services retrieved from the service provider across tests, as they might be stateful.

#### Simple test setup
Let the test initialization procedure create the services based on the registered services. 

```csharp
public class HealthChecksIntegrationTest : IntegrationTestBase<HealthChecksIntegrationTest>
{
    [Fact]
    public void COllectHealthChecks()
    {
        var healthChecks = ServiceProvider.GetRequiredService<ILazyEnumerable<IHealthCheck, HealthCheckMetadata>>();
        healthChecks.Select(hc => hc.Metadata).Should().Satisfy(
            e => e.ServiceKey == "AppModules" && e.FailureStatus == HealthStatus.Degraded,
            e => e.ServiceKey == "Memory" && e.FailureStatus == HealthStatus.Unhealthy,
            e => e.ServiceKey == "Process" && e.FailureStatus == HealthStatus.Unhealthy
        );
    }
}
```

#### Simple test setup with additional services from the test assembly

```csharp
public class MediatorServiceRegistrationIntegrationTest()
    : IntegrationTestBase<MediatorServiceRegistrationIntegrationTest>(includeTestAssembly: true)
{
    //...
}
```

## Other resources

* [Kephas.DependencyInjection](https://www.nuget.org/packages/Kephas.DependencyInjection)
* [Kephas.Testing.Xunit](https://www.nuget.org/packages/Kephas.Testing.Xunit)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
