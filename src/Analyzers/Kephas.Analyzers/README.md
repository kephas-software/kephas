# Analyzers

## Introduction
Provides analysis of Kephas based assemblies and code generation.

Typically used areas and classes/interfaces/services:
* ``AppServicesSourceGenerator``, ``ModuleInitializerSourceGenerator``.

## Application services

Generates code for application services registered through `AppServiceContract` attributes.

## Assembly initializers

Generates code for assembly initializers identified by classes implementing the `IAssemblyInitializer` interface.

## Reflection assembly initializers

Generates code for automatic registration of runtime element information factories into the `RuntimeTypeRegistry.Instance`.

## Other resources

* [Kephas.Abstractions](https://www.nuget.org/packages/Kephas.Abstractions)
* [Kephas.Services](https://www.nuget.org/packages/Kephas.Services)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
