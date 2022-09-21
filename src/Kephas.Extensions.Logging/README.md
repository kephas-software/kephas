# Logging (Microsoft.Extensions.Logging)

## Introduction

Provides the [Microsoft.Extensions.Logging](https://www.nuget.org/packages/Microsoft.Extensions.Logging) logging services implementation.

## Usage
There are basically two scenarios how Kephas and Microsoft.Extensions.Logging can be integrated. Either has upsides and downsides.
> WARNING: Do not use both <see cref="WithExtensionsLogManager"/> and <see cref="UseKephasLogging"/> methods 
> as this might generate StackOverflow exception.

### Configure logging in Kephas, redirect from Microsoft.Extensions.Logging.

This scenario should be considered when the logging in configured in the services collection, and the configured log manager is used to output the logging from `Microsoft.Extensions.Logging`.
* Advantage: The loggers are available also before the container is built.
* Disadvantage: Kephas will log only to its own configured logger, will ignore the logger configured with `Microsoft.Extensions.Logging`.

```csharp
// 1. configure in ambient services
appServiceCollection.WithSerilogManager();

// 2. use in Microsoft.Extensions.Logging
servicesCollection.UseKephasLogging(appServiceCollection.GetServiceInstance<ILogManager>());
```
### Configure logging in `Microsoft.Extensions.Logging`, redirect logging from Kephas.

```csharp
appServiceCollection.WithExtensionsLogManager(services);
```

## Other resources

* [Kephas.Abstractions](https://www.nuget.org/packages/Kephas.Abstractions)
* [Kephas.Logging](https://www.nuget.org/packages/Kephas.Logging)
* [Kephas.Extensions.DependencyInjection](https://www.nuget.org/packages/Kephas.Extensions.DependencyInjection)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
