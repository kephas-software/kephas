# Dependency Injection (```Autofac```)

## Introduction
Provides a dependency injection implementation based on the ```Autofac``` infrastructure.

Typically used areas and classes/interfaces/services:
* `IAppServiceCollection.BuildWithAutofac()`.

## Usage

```csharp
var serviceProvider = new AppServiceCollection().BuildWithAutofac();
```

## Other resources

* [Kephas.Services](https://www.nuget.org/packages/Kephas.Services)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
