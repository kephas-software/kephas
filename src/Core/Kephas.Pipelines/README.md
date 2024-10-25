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

* [Kephas.DependencyInjection](https://www.nuget.org/packages/Kephas.DependencyInjection)
* [Kephas.Reflection](https://www.nuget.org/packages/Kephas.Reflection)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
