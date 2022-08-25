# 12.0.0

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

# Other resources
Please check https://github.com/kephas-software/kephas/releases for the change log.
Also check the documentation and the samples from https://github.com/kephas-software/kephas/wiki and https://github.com/kephas-software/kephas/tree/master/Samples.
