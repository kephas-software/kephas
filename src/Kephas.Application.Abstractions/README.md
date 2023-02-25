# Abstractions (Application)

## Introduction
This package provides abstractions for applications based on Kephas Framework.

Check the following packages for more information:
* [Kephas.Core](https://www.nuget.org/packages/Kephas.Core)
* [Kephas.Services](https://www.nuget.org/packages/Kephas.Services)

Typically used areas and classes/interfaces/services:
* Application management: ``StaticAppRuntime``, ``DynamicAppRuntime``, ``IAppLifecycleBehavior``.

## The `IAppRuntime` service

The application runtime information is available through the `IAppRuntime` service.
This service is configured during the application setup phase.

### Configuring the application runtime

#### Setting up the filter for application-specific assemblies
From all the assemblies loaded at runtime, only a subset are application-specific, the rest being provided by the infrastructure.
To filter out these assemblies, use the `OnIsAppAssembly` setup method in combination with the `IsAppAssembly` checking method counterpart.  

```csharp

// configure the app assemblies
ambientServices.WithStaticAppRuntime(config: rt => rt.OnIsAppAssembly(an => !this.IsTestAssembly(an)));

// consume app assemblies
var appRuntime = ambientServices.GetAppRuntime();
Assert.IsTrue(appRuntime.IsAppAssembly(new AssemblyName("My.Tests")));
Assert.IsFalse(appRuntime.GetAppAssemblies().Any(a =-> a.Name.Contains("Test")));
```

This feature could also be used to exclude all assemblies that do not have a proper signature, like a public key token.

> Notes: For example, only the application-specific assemblies are used for discovering application services or messages.

## Other resources

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
