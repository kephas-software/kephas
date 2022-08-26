﻿# 12.0.0

* Moved ```ActivationException``` and ```ImplementationForAttribute``` to [Kephas.Reflection](https://www.nuget.org/packages/Kephas.Reflection).
* Added ```release-notes.md```.
* The following methods in ```IAppRuntime``` are methods with default implementation instead of extension methods:
  * ```IsRoot```. Breaking change: This method was transformed to a property.
  * ```GetAppId```.
  * ```GetAppVersion```.
  * ```GetAppIdentity```.
  * ```GetEnvironment```.
  * ```IsDevelopment```. Breaking change: Named changed to ```IsDevelopmentEnvironment```.
  * ```GetAppInstanceId```.
  * ```GetFullPath```.
* Added ```Expando<T>```, which introduces the possibility of supporting ```IDictionary<string, T>```.
* Breaking change: ```Expando``` does not offer anymore the possibility of providing a dictionary or object in the constructor. Instead, use the ```object.ToExpando()``` extension method or specialize your own ```Expando<T>/Expando<object?>```.

# Other resources
Please check https://github.com/kephas-software/kephas/releases for the change log.
Also check the documentation and the samples from https://github.com/kephas-software/kephas/wiki and https://github.com/kephas-software/kephas/tree/master/Samples.