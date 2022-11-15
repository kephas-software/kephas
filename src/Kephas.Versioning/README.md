# Versioning

## Introduction
Provides support for semantic versioning.

Typically used areas and classes/interfaces/services:
* ```SemanticVersion```, ```VersionRange```.

## Semantic versioning

This package implements the specification [SemVer 2.0](https://semver.org).

### SemanticVersion

```csharp
var version = SemanticVersion.Parse("2.1.4-dev.12+1234");

Assert.AreEqual(2, version.Major);
Assert.AreEqual(1, version.Minor);
Assert.AreEqual(4, version.Patch);
Assert.AreEqual("dev", version.ReleaseLabels.First());
Assert.AreEqual("12", version.ReleaseLabels.Skip(1).First());
Assert.AreEqual("1234", version.Metadata);
```

### VersionRange

```csharp
var range = VersionRange.Parse("2.*:");

Assert.IsNull(range.MaxVersion);
Assert.IsNotNull(range.MinVersion);
Assert.AreEqual(range.MinVersion, SemanticVersion.Parse("2.0.0"));
```

## Other resources

* [Kephas.Abstractions](https://www.nuget.org/packages/Kephas.Abstractions)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
