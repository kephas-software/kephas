# Release notes

* Change log: https://github.com/kephas-software/kephas/releases.
* Documentation and samples: https://github.com/kephas-software/kephas/wiki and https://github.com/kephas-software/kephas/tree/master/Samples.

# 12.0.0

* General
  * Added ```release-notes.md```.

* Activation
  * Moved ```ActivationException``` and ```ImplementationForAttribute``` to [Kephas.Reflection](https://www.nuget.org/packages/Kephas.Reflection).

* Application 
  * The following methods in ```IAppRuntime``` are methods with default implementation instead of extension methods:
    * ```IsRoot```. Breaking change: This method was transformed to a property.
    * ```GetAppId```.
    * ```GetAppVersion```.
    * ```GetAppIdentity```.
    * ```GetEnvironment```.
    * ```IsDevelopment```. Breaking change: Named changed to ```IsDevelopmentEnvironment```.
    * ```GetAppInstanceId```.
    * ```GetFullPath```.
  * Breaking change: Removed the filter parameter from ``IAppRuntime.GetAppAssemblies()``. Reason: was not used.
  * Breaking change: Removed ``IAppRuntime.LoadAssembly*`` methods, used only in the implementation.
  * NEW: added support for module initialization. Check the [README.md](README.md) file for more information.

* Versioning
  * Breaking change: Moved ```Versioning``` namespace to ```Kephas.Versioning``` package.

* Linq
  * Breaking change: Removed ```IDisposableQueryable```, not feasible.

* Text.Encoding
  * Breaking change: Removed obsolete ```Base64Data```.
