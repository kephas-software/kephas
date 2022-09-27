# Release notes

* Change log: https://github.com/kephas-software/kephas/releases.
* Documentation and samples: https://github.com/kephas-software/kephas/wiki and https://github.com/kephas-software/kephas/tree/master/Samples.

# 12.0.0

## Ambient services
* Breaking change: Removed `IAmbientServicesMixin`.
* Breaking change: Removed `IAmbientServices.WithInjector(injector)`.
* Breaking change: Renamed `IAmbientServices.WithInjector<TInjectorBuilder>(builderOptions)` to `IAmbientServices.BuildWith<TInjectorBuilder>(builderOptions)`.
* Breaking change: Renamed `IAmbientServices` to `IAppServiceCollection` and `AmbientServices` to `AppServiceCollection`.
Also, their semantic was changed to only hold the collection of services, not cumulate also the service provider functionality.
* Breaking change: Removed `IAmbientServices.GetAppAssemblies()`. Use instead `IAppServiceCollection.GetAppRuntime().GetAppAssemblies()`.
* NEW: Added `AddCollector(collect)` and `Initialize(appServiceCollection)` static methods in `IAppServiceCollection`.
* NEW Added `TryGetServiceInstance` and `GetServiceInstance` extension methods for `IAppServiceCollection`, for trying to retrieve services registered as instances before the service provider is built.
* Breaking change: Removed the constructors of `AppServiceCollection` (former `AmbientServices`) accepting `IRuntimeTypeRegistry`. Instead, register the runtime type registry after creating the instance.
* Breaking change: `IAmbientServices.RegisterService` renamed to `IAppServiceCollection.Add`.
* Breaking change: `IAmbientServices.IsRegistered` renamed to `IAppServiceCollection.Contains`.
* NEW: Added `IAmbientServices.Replace`.
* Breaking change: removed `SetAppServiceInfos` and `GetAppServiceInfos`. Instead, `IAppServiceCollection` is now the service enumeration.

OLD code
```csharp
new AmbientServices(typeRegistry: new RuntimeTypeRegistry());
```
NEW code
```csharp
new AppServiceCollection().Register<IRuntimeTypeRegistry>(new RuntimeTypeRegistry(), b => b.ExternallyOwned());
```

* Breaking change: removed `Injector` and `LogManager` properties from `IAmbientServices`. There is no replacement, as the injector/service provider is built at a later time and is not registered in the service collection.
* Breaking change: removed `LogManager` property from `IAmbientServices`. Instead, use one of the new `TryGetServiceInstance`/`GetServiceInstance` extension methods. 

OLD code
```csharp
var logManager = AmbientServices.LogManager;
```
NEW code
```csharp
var logManager = AppServiceCollection.TryGetServiceInstance<ILogManager>();
```

## Export factory
* Breaking change: Removed `ExportFactory(Func<Tuple<TContract, Action>> factory)` constructor.
* Breaking change: Removed `IExportFactory.CreateExport`. Replace with `IExportFactory.CreateExportedValue`.
* Breaking change: Removed `IExport` and `Export`. No replacement. Reason: obsolete artifact to support `System.Composition`.

## Ordered service collections
* Breaking change: Renamed `IOrderedLazyServiceCollection` to `ILazyEnumerable` and `IOrderedServiceFactoryCollection` to `IFactoryEnumerable`. Renamed `GetServices` in both to `SelectServices`. Added `GetService` and `TryGetService` methods in both.
* Breaking change: Renamed `IEnabledServiceCollection` to `IEnabledEnumerable` to preserve consistency.
* Breaking change: Renamed `IEnabledLazyServiceCollection` to `IEnabledLazyEnumerable` to preserve consistency.
* Breaking change: Renamed `IEnabledServiceFactoryCollection` to `IEnabledFactoryEnumerable` to preserve consistency.
* NEW: Added `IInjectionBuildContext.GetAppAssemblies(): IEnumerable<Assembly>`.

## Context
* Breaking change: removed `IContext.AmbientServices`. Reason: the injector/service provider should suffice. If required, the injector can be invoked to resolve the `IAppServiceCollection` service.

## Injection
* Breaking change: `IInjector` interface removed, replaced by `IServiceProvider`.
`ServiceProviderInjectionExtensions` will provide the methods of the `IInjector` interface as extension methods of `IServiceProvider`.
* Breaking change: All `Injector` methods were renamed to `ServiceProvider`.
* IInjector.CreateScope()