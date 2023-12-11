# Release notes

* Change log: https://github.com/kephas-software/kephas/releases.
* Documentation and samples: https://github.com/kephas-software/kephas/wiki and https://github.com/kephas-software/kephas/tree/master/Samples.

# 12.0.0

* NEW: Created ```Kephas.Versioning``` package - split from ```Kephas.Abstractions```. ```ActivationException``` and ```ImplementationForAttribute``` to [Kephas.Reflection](https://www.nuget.org/packages/Kephas.Reflection).
* Breaking change: ```VersionComparer.GetHashCode(version)``` uses now ```System.HashCode``` to compute the version hash code.
