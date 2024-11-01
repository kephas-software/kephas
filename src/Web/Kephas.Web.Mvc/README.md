# Web Extensions for MVC

This NuGet package provides MVC extensions used by web projects.

## Controllers configuration

* Example

```csharp
services.ConfigureControllers();
// note: do not use services.AddControllers() anymore.
```

### `IApplicationPartsProvider` service
This service will provide all the parts containing controllers. The default implementation `ApplicationPartsProvider` will use
the `IAppRuntime` service to identify all application assemblies and register them. Register your own implementation prior to calling
`services.ConfigureControllers()` to have a different behavior.

# Other resources

* Please check the [release notes](release-notes.md) file in this package.

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
