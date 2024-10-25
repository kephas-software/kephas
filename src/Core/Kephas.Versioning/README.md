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

Assert.Equal(2, version.Major);
Assert.Equal(1, version.Minor);
Assert.Equal(4, version.Patch);
Assert.Equal("dev", version.ReleaseLabels.First());
Assert.Equal("12", version.ReleaseLabels.Skip(1).First());
Assert.Equal("1234", version.Metadata);
```

### VersionRange

```csharp
var range = VersionRange.Parse("2.*:");

Assert.Null(range.MaxVersion);
Assert.NotNull(range.MinVersion);
Assert.Equal(range.MinVersion, SemanticVersion.Parse("2.0.0"));
```

## Other resources


> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
