# Dependency Injection (```Microsoft.Extensions.DependencyInjection```)

## Introduction
Provides a dependency injection implementation based on the ```Microsoft.Extensions.DependencyInjection``` infrastructure.

Typically used areas and classes/interfaces/services:
* `IAppServiceCollection.BuildWithDependencyInjection()`.
* `IServiceCollection.AddFromServicesBuilder()`.

## Usage

### Create a service provider from merging into the service collection 

```csharp
var builder = new AppServiceCollectionBuilder();
var services = new ServiceCollection();
var serviceProvider = builder.BuildWithDependencyInjection(services);
```

### Add collected services to the service collection

```csharp
var builder = new AppServiceCollectionBuilder();
var services = new ServiceCollection()
    .AddFromServicesBuilder(builder);
```

## Other resources

* [Kephas.Services](https://www.nuget.org/packages/Kephas.Services)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
