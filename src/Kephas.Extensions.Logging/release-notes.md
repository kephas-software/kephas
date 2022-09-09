# Release notes

* Change log: https://github.com/kephas-software/kephas/releases.
* Documentation and samples: https://github.com/kephas-software/kephas/wiki and https://github.com/kephas-software/kephas/tree/master/Samples.

# 12.0.0

* Added ``README.md`` and ``release-notes.md``.
* Breaking change: `LoggingServiceCollectionExtensions` renamed to `ExtensionsLogging`.
* Breaking change: Renamed `ConfigureExtensionsLogging` to `UseKephasLogging` and inverted the parameters.
* Breaking change: Added `IServicesCollection` parameter to `IAmbientServices.WithExtensionsLogManager` extension method.
* Breaking change: Removed the `LoggerFactory`, use the default one from `Microsoft.Extensions.Logging`.
