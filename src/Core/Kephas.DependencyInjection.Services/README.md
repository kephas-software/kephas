# Dependency Injection (Services)

## Introduction
This package provides the primary infrastructure for defining and collecting application services.

## Application services

### The anatomy
An application service has three main parts:
* The *service contract*
    * This is the service type, providing at the same time:
        * the service API, to access the functionality, and
        * the service key.
    * Typically, it is an interface, but it can also be an abstract class.
    * It is strongly recommended to be as abstract as possible (interfaces preferred), as it leverages unit testing
      and decouples the system.
* The *service implementation*
    * This is a class implementing or specializing the service type.
    * It receives all the dependencies through the constructor.
    * There can be one or more service implementations for a service type.
* *Metadata*
    * Provides additional structured information about the service.
    * Can be associated both to the service contract and to the service implementation.
    * Some typical usage:
        * Filter the services based on the metadata.
        * Support cross-cutting concerns, or decision-making patterns.

### The role Kephas plays in the Dependency Injection

Typically, the application services are not consumed directly, but through some kind of dependency injection infrastructure.
To name some:
* [Autofac](https://autofac.org/)
* [Microsoft.Extensions.DependencyInjection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-usage)

To be able to use application services, the steps below must be followed:
* collect and register.
* build the DI container.
* retrieve the services from the container and invoke them.

Various containers take slightly different approaches for these steps. So, what does Kephas do with respect to the dependency injection?
* helps the collecting and the registration procedures, so that the code stays clean and maintainable.
* tweaks the service retrieval, with the help of metadata, to leverage the modularization and extensibility of the application ecosystem.

> Note: Kephas does not provide a full DI solution, there are plenty of excellent products available.
> Instead, it focuses on enhancing these products with useful additional features.

## Service metadata

### Service contract metadata at declaration

* `ServiceType`: the key for identifying the service implementations and, at the same time, the service API.
* `Lifetime`: `Transient`, `Scoped`, or `Singleton`.
* `MetadataType`: the type used to store metadata, typically specializing `ServiceMetadata`.
* `AsOpenGeneric`: indicates whether an open generic service should be registered as open generic,
or instead constructed service implementation are expected.

#### Example: Simple service declaration

```csharp
[TransientService]
public interface IJsonModifier
{
    void Modify(JsonTypeInfo jsonTypeInfo);
}

// in this example, we have the following metadata:
// ServiceType = typeof(IJsonModifier)
// Lifetime = ServiceLifetime.Transient
// MetadataType = ServiceMetadata
// AsOpenGeneric = false
```

#### Example: Generic service declaration with constructed generic implementations

* `IAsyncPipelineBehavior`: Service type interface to help collect all behaviors and then filter only the appropriate ones.
  * Also the DI key for collecting instances at runtime.
* `IAsyncPipelineBehavior<in TTarget, in TOperationArgs, out TResult>`: Service declaration interface, used to collect generic parameter metadata
and provide typed parameters.

```csharp
public interface IAsyncPipelineBehavior;

[ScopedService(ServiceType = typeof(IAsyncPipelineBehavior))]
public interface IAsyncPipelineBehavior<in TTarget, in TArgs, TResult> : IAsyncPipelineBehavior
{
    Task<TResult> InvokeAsync(
        Func<Task<TResult>> next,
        TTarget target,
        TArgs args,
        CancellationToken cancellationToken);
}

// in this example, we have the following metadata:
// ServiceType = typeof(IAsyncPipelineBehavior)
// Lifetime = ServiceLifetime.Scoped
// MetadataType = ServiceMetadata
// AsOpenGeneric = false
```

#### Example: Open generic service declaration 

```csharp
[TransientService(RegistrationKind.AsOpenGeneric)]
public interface ILazyEnumerable<TService>
    : IEnumerable<Lazy<TService, ServiceMetadata>>
{
}

// in this example, we have the following metadata:
// ServiceType = typeof(ILazyEnumerable<>)
// Lifetime = ServiceLifetime.Transient
// MetadataType = ServiceMetadata
// AsOpenGeneric = true
```

### Service implementation metadata

The built-in supported metadata is provided by the `ServiceMetadata` record, or one specializing it.
The following values are provided:
* `Priority`: the service priority, used to order the services.
* `ServiceKey`: the service key, used to identify the service implementation.
  * Typically, this is the name of the service implementation type from which the service type name has been stripped away.
* `OverrideBaseImplementation`: True if the service overrides the service it specializes, false otherwise.
* `ImplementationType`: the concrete service type implementing the service contract.

#### Example: Service implementation metadata

```csharp
// The definition of the Health Check metadata 
public record HealthCheckMetadata : ServiceMetadata
{
    public HealthCheckMetadata(IDictionary<string, object?>? metadata)
        : base(metadata)
    {
        if (metadata == null)
        {
            return;
        }

        FailureStatus = (HealthStatus)Convert.ChangeType(TryGetValue(metadata, nameof(FailureStatus), HealthStatus.Healthy)!, typeof(int));
        Timeout = (TimeSpan?)TryGetValue(metadata, nameof(Timeout));
        Tags = (IEnumerable<string>)TryGetValue(metadata, nameof(Tags), null)!;
    }

    public HealthStatus? FailureStatus { get; }

    public TimeSpan? Timeout { get; }
    
    public IEnumerable<string>? Tags { get; }
}

// attribute used to collect the Tags metadata
[AttributeUsage(AttributeTargets.Class)]
public sealed class TagsAttribute(params string[] value)
    : ServiceMetadataValueAttributeBase<IEnumerable<string>>(value);

// The service implementing IHealthCheck and using the metadata attributes
[FailureStatus(HealthStatus.Unhealthy)]
[Tags("process")]
public class ProcessHealthCheck(IAppRuntime appRuntime) : IHealthCheck
{
    //...
}
```

## Declarative service registration

### Services declared within the application scope

### Service declared outside the application scope

#### Single services

These services are declared using one of `[DeclareTransientService]`, `[DeclareScopedService]`, or `[DeclareSingletonService]`
attributes at assembly level.
This is useful when the service to be annotated is not accessible for modification, for example when defined in a third party assembly.

```csharp
[assembly: DeclareTransientService(typeof(ILazyEnumerable<>), typeof(LazyEnumerable<>))]
[assembly: DeclareSingletonService<TimeProvider, DefaultTimeProvider>]
```

#### Service families

These services are declared using one of `[DeclareTransientServiceFamily]`, `[DeclareScopedServiceFamily]`, or `[DeclareSingletonServiceFamily]`
attributes at assembly level. All non-abstract types derived from the service type will be marked as service implementations (the service family).
This is useful when the service to be annotated is not accessible for modification, for example when defined in a third party assembly.
> Note: Open generics are not supported when declaring service families. In these cases, declare the services using
> one of `[DeclareTransientService]`, `[DeclareScopedService]`, or `[DeclareSingletonService]`.

```csharp
[assembly: DeclareTransientServiceFamily(typeof(IRequestHandler<>), MetadataType = typeof(RequestResponseMetadata))]
```

## Other resources

* [Kephas.DependencyInjection.Services](https://www.nuget.org/packages/Kephas.DependencyInjection.Services)
* [Kephas.Extensions.DependencyInjection](https://www.nuget.org/packages/Kephas.Extensions.DependencyInjection)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.

