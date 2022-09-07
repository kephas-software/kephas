# Release notes

* Change log: https://github.com/kephas-software/kephas/releases.
* Documentation and samples: https://github.com/kephas-software/kephas/wiki and https://github.com/kephas-software/kephas/tree/master/Samples.

# 12.0.0

* Breaking change: Removed ```ExportFactory(Func<Tuple<TContract, Action>> factory)``` constructor.
* Breaking change: Removed ```IExportFactory.CreateExport```. Replace with ```IExportFactory.CreateExportedValue```.
* Breaking change: Removed ```IExport``` and ```Export```. No replacement. Reason: obsolete artifact to support ``System.Composition``.
* Breaking change: Renamed ``IOrderedLazyServiceCollection`` to ``ILazyEnumerable`` and ``IOrderedServiceFactoryCollection`` to ``IFactoryEnumerable``. Renamed ``GetServices`` in both to ``SelectServices``. Added ``GetService`` and ``TryGetService`` methods in both.
* Breaking change: Renamed ``IEnabledServiceCollection`` to ``IEnabledEnumerable`` to preserve consistency.
* Breaking change: Renamed ``IEnabledLazyServiceCollection`` to ``IEnabledLazyEnumerable`` to preserve consistency.
* Breaking change: Renamed ``IEnabledServiceFactoryCollection`` to ``IEnabledFactoryEnumerable`` to preserve consistency.
* Breaking change: Removed ``IAmbientServices.GetAppAssemblies()``. Use instead ``IAmbientServices.GetAppRuntime().GetAppAssemblies()``.
* NEW: Added ``IInjectionBuildContext.GetAppAssemblies(): IEnumerable<Assembly>``.

