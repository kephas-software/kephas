# Release notes

* Change log: https://github.com/kephas-software/kephas/releases.
* Documentation and samples: https://github.com/kephas-software/kephas/wiki and https://github.com/kephas-software/kephas/tree/master/Samples.

# 12.0.0

* Added ``README.md`` and ``release-notes.md``.
* Breaking change: removed ``ILicenceCheckResult`` and ``LicenceCheckResult``. Replaced by ``IOperationResult<bool>``. 
* Breaking change: ``ILicensingManager.CheckLicense`` and ``CheckLicenseAsync`` now return an ``IOperationResult<bool>`` instead of ``ILicenceCheckResult``.
* NEW: ``IAppRuntime.OnCheckLicense`` registers a ``CheckLicense`` callback.
  * Example:
```csharp
// register the callback 
appRuntime.OnCheckLicense((appid, context) => licensingManager.CheckLicense(appid, context));

// invoke the license check
appRuntime.CheckLicense(new AppIndentity("my-app"), null);
```