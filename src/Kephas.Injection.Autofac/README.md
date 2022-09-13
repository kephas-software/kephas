# Dependency Injection (```Autofac```)

## Introduction
Provides a dependency injection implementation based on the ```Autofac``` infrastructure.

Typically used areas and classes/interfaces/services:
* ```IAmbientServices.BuildWithAutofac()```.

## Usage

```csharp
var ambientServices = new AmbientServices().BuildWithAutofac();
```

## Other resources

* [Kephas.Injection](https://www.nuget.org/packages/Kephas.Injection)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
