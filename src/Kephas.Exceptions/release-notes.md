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

* Dynamic
  * Breaking change: ```ExpandoBase<T>``` is now generic, introducing the possibility of supporting ```IDictionary<string, T>```.
  * Breaking change: ```Expando``` does not offer anymore the possibility of providing a dictionary or object in the constructor. Instead, use the ```object.ToExpando()``` extension method or specialize ```ExpandoBase<T>``` with your own implementation.
  * Breaking change: ```IExpandoBase``` interface was removed, ```HasDynamicMember``` and ```ToDictionary``` methods were merged into ```IDynamic``` with default implementation.
  * Breaking change: ```obj.ToExpando()``` extension method marked as obsolete. Replaced by ```ToDynamic()```.
  * NEW: ``ToDynamic()`` supports now also dynamic objects.

* Reflection
  * NEW: Added ```ReflectionHelper.GetValue``` and ```ReflectionHelper.SetValue``` methods to access in a performant way object properties over reflection.

* Versioning
  * Breaking change: Moved ```Versioning``` namespace to ```Kephas.Versioning``` package.

* Threading
  * Breaking change: Removed the ```throwOnTimeout``` parameter from ```WaitNonLocking()``` and ```GetResultNonLocking()```.
  * NEW: added ```TryWaitNonLocking()``` and ```TryGetResultNonLocking()``` methods for silent fail cases.
  * Added ThreadContextAssemblyInitializer to support restoring the culture in the thread when returning from async methods.

* Linq
  * Breaking change: Removed ```IDisposableQueryable```, not feasible.

* Text.Encoding
  * Breaking change: Removed obsolete ```Base64Data```.
