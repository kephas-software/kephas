# Release notes

* Change log: https://github.com/kephas-software/kephas/releases.
* Documentation and samples: https://github.com/kephas-software/kephas/wiki and https://github.com/kephas-software/kephas/tree/master/Samples.

# 12.0.0
* Initial branch from `Kephas.Abstractions`.
* Breaking change: Removed the ```throwOnTimeout``` parameter from ```WaitNonLocking()``` and ```GetResultNonLocking()```.
* NEW: added ```TryWaitNonLocking()``` and ```TryGetResultNonLocking()``` methods for silent fail cases.
* Added ThreadContextAssemblyInitializer to support restoring the culture in the thread when returning from async methods.
