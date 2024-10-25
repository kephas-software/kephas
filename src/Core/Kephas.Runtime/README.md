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

## Other resources

* [Kephas.DependencyInjection](https://www.nuget.org/packages/Kephas.DependencyInjection)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.

