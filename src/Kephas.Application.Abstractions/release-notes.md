# Release notes

* Change log: https://github.com/kephas-software/kephas/releases.
* Documentation and samples: https://github.com/kephas-software/kephas/wiki and https://github.com/kephas-software/kephas/tree/master/Samples.

# 12.0.0

* Added ``README.md`` and ``release-notes.md``.
* Breaking change: Removed the filter parameter from ``AppRuntimeBase.GetAppAssemblies()``. Reason: was not used.
* Breaking change: Moved ``IAmbientServices.GetAppRuntime()`` extension method from ``ApplicationAmbientServicesExtensions`` to ``InjectionAmbientServicesExtensions``.
* Breaking change: Moved the ``Licensing`` area to its own package: [Kephas.Licensing](https://www.nuget.org/packages/Kephas.Licensing).