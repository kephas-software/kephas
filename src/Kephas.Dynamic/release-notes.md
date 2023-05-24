# Release notes

* Change log: https://github.com/kephas-software/kephas/releases.
* Documentation and samples: https://github.com/kephas-software/kephas/wiki and https://github.com/kephas-software/kephas/tree/master/Samples.

# 12.0.0
* Initial split from `Kephas.Abstractions`.
* Breaking change: `ExpandoBase<T>` is now generic, introducing the possibility of supporting `IDictionary<string, T>`.
* Breaking change: `Expando` does not offer anymore the possibility of providing a dictionary or object in the constructor. Instead, use the `object.ToExpando()` extension method or specialize `ExpandoBase<T>` with your own implementation.
* Breaking change: `IExpandoBase` interface was removed, `HasDynamicMember` and `ToDictionary` methods were merged into `IDynamic` with default implementation.
* Breaking change: `obj.ToExpando()` extension method marked as obsolete. Replaced by `ToDynamic()`.
* NEW: ``ToDynamic()`` supports now also dynamic objects.
* NEW: Added `DynamicHelper.GetValue` and `DynamicHelper.SetValue` methods to access in a performant way object properties over reflection.

