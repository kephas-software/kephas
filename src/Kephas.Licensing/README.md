# Licensing

## Introduction

Provides the infrastructure for licensing an application or parts of it.

Typically used areas and classes/interfaces/services:
* ``ILicensingManager``.

## Usage

#### Example with the default licensing manager

```csharp
ambientServices.WithDefaultLicensingManager();
```

#### Example with a custom licensing manager

```csharp
public class CustomLicensingManager : ILicensingManager
{
    // ...
}

ambientServices.WithLicensingManager(new CustomLicensingManager());
```

#### Example registering a custom license check callback

```csharp
// register the callback 
appRuntime.OnCheckLicense((appid, context) => return new OperationResult<bool>(true));

// invoke the license check
appRuntime.CheckLicense(new AppIdentity("my-app"), null);
```

## The ``DefaultLicensingManager``

This service is the default implementation of the ``ILicensingManager`` application service contract. It uses an ``ILicenseRepository`` service for the license persistence. 

## Other resources

* [Kephas.Abstractions](https://www.nuget.org/packages/Kephas.Abstractions)
* [Kephas.Services](https://www.nuget.org/packages/Kephas.Services)
* [Kephas.Operations](https://www.nuget.org/packages/Kephas.Operations)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
