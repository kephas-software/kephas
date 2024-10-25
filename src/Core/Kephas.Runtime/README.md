# Runtime

## Introduction
This package provides infrastructure for runtime information and operations.

## Assembly initialization

Tme [module initializer feature](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/module-initializers?source=recommendations) is supported starting with .NET 5 and C# 9.
To leverage it, Kephas introduced the `IAssemblyInitializer` interface and the `AssemblyInitializerAttribute`, which can be used to perform assembly initialization upon loading.
The [Kephas.Analyzers](https://www.nuget.org/packages/Kephas.Analyzers) package is aware of these and generates the code according to the following rules:
* A class is generated with a single `InitializeModule` method, annotated appropriately.
```csharp
internal static class ModuleInitializer_My_Assembly
{
    [ModuleInitializer]
    internal static void InitializeModule()
    {
        // ...
    }
}
```
* All classes implementing the `IAssemblyInitializer` interface are instantiated and their `Initialize` method is called.
    * It is by-design to have instance classes instead of static classes.
* All `AssemblyInitializerAttribute`s are collected from the executing assembly, their `InitializerTypes` iterated, instantiated, and their `Initialize` method called.
    * Typically, the `AssemblyInitializerAttribute` is used by source generators to provide at runtime assembly initializers not available at code generation time, like those defined by code generators.

> Recommendation: Prefer using statically invoked assembly initializers instead by using the `AssemblyInitializerAttribute`.
> This is solely for increasing loading time performance through avoiding reflection.

> Caution: do not add to an assembly referencing the [Kephas.Analyzers](https://www.nuget.org/packages/Kephas.Analyzers) package
> a `[ModuleInitializer]` annotated method, as it will collide with the one generated.

## The application runtime
### `IAppRuntime`

Service contract providing runtime information:
* `GetAppAssemblies()`: Gets the application specific assemblies. By default, this excludes system and test assemblies.
* `GetEntryAssembly()`: Gets the entry assembly.
* `GetAppName()`: Gets the application name.
* `GetAppVersion()`: Gets the application version.
* `GetAppInstanceId()`: Gets the ID of the running application instance.
* `GetEnvironment()`: Gets the environment name.

### `AppRuntime`
The default implementation of `IAppRuntime`. When instantiating this, `AppRuntimeSettings` can be also provided with the following information:
* `EntryAssembly`: the entry assembly used for computing the application assemblies. It is initialized with `Assembly.GetEntryAssembly()`.
* `AppAssemblyNamePattern`: the regex pattern used to identify the application assemblies. By default, it excludes system assemblies and those ending with `Test` or `Tests`.
* `AppName`: the application name. If this value is not provided, it is retrieved from the `AssemblyTitle` attribute.
* `Environment`: the environment name. If this value is not provided, it is retrieved from the `AssemblyInformationalVersion`, `AssemblyFileVersion`, or `AssemblyVersion` attributes, whichever one has a value.

> Note: Typically, you will get this service over dependency injection.

* Example

```csharp
IAppRuntime appRuntime = new AppRuntime();
var appAssemblies = appRuntime.GetAppAssemblies(); 
```

## Other resources

* [Kephas.DependencyInjection](https://www.nuget.org/packages/Kephas.DependencyInjection)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.

