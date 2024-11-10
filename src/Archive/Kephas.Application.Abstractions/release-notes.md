# Release notes

* Change log: https://github.com/kephas-software/kephas/releases.
* Documentation and samples: https://github.com/kephas-software/kephas/wiki and https://github.com/kephas-software/kephas/tree/master/Samples.

# 12.0.0

* Added ``README.md`` and ``release-notes.md``.
* Breaking change: Removed the filter parameter from ``AppRuntimeBase.GetAppAssemblies()``. Reason: was not used.
* Breaking change: Moved ``IAppServiceCollection.GetAppRuntime()`` extension method from ``ApplicationAmbientServicesExtensions`` to ``InjectionAmbientServicesExtensions``.
* Breaking change: Moved the ``Licensing`` area to its own package: [Kephas.Licensing](https://www.nuget.org/packages/Kephas.Licensing).
* Breaking change: ``AppRuntimeBase`` will not use the ``checkLicence`` callback in the constructor. Instead, the ``With*LicenceManager`` extension methods will add the check licence callback in the ``Kephas.Licensing`` package.
  * Example:
```csharp
// custom licensing manager
var licensingManager = new MyLicensingManager();

// custom and setup
ambientServices.Register(licensingManager);
ambientServices.GetAppRuntime()
    .OnCheckLicense((appid, context) => licensingManager.CheckLicense(appid, context));

// alternative to the previous block, use the WithLicensingManager() extension method
ambientServices.WithLicensingManager(licensingManager);
```
* Breaking change: ``AppRuntimeBase`` will not use the ``assemblyFilter`` callback in the constructor. Instead, use the ``OnIsAppAssembly`` extension methods to specify the callback used for checking whether an assembly is an application-specific assembly.
  * Example
```csharp
ambientServices.WithStaticAppRuntime(config: rt => rt.OnIsAppAssembly(an => !this.IsTestAssembly(an)));

Assert.IsTrue(ambientServices.GetAppRuntime().IsAppAssembly(new AssemblyName("My.Tests")));
```
* Breaking change: `AppRuntimeBase` will not use the `getLocations` callback in the constructor. Instead, use the ``OnGetLocations`` extension methods to specify the callback used for getting the locations.