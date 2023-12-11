# Pipelines

## Introduction
Provides abstractions for intercepting operations to apply transversal behavior.

Typically used areas and classes/interfaces/services:
* `IPipeline`, `IPipelineBehavior`.

## Usage

```csharp

var pipeline = serviceProvider.GetRequiredService<IPipeline<>>

```

## Other resources

* [Kephas.Core](https://www.nuget.org/packages/Kephas.Core)
* [Kephas.Services](https://www.nuget.org/packages/Kephas.Services)
* [Kephas.Reflection](https://www.nuget.org/packages/Kephas.Reflection)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
