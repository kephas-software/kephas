# Release notes

* Change log: https://github.com/kephas-software/kephas/releases.
* Documentation and samples: https://github.com/kephas-software/kephas/wiki and https://github.com/kephas-software/kephas/tree/master/Samples.

# 12.0.0

* NEW: Added ```serviceCollection.AddGenericCollections()``` extension method to provide the infrastructure for Kephas integration with the ```Microsoft.Extensions.DependencyInjection```.
* NEW: Added ```README.md``` and ```release-notes.md```.
* Breaking change: `IAmbientServices.WithServiceCollection(services)` refactored to `IServiceCollection.UseAppServices(appServices)`.
