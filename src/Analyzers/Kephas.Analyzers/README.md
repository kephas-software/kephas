# Analyzers

## Introduction
Provides analysis of Kephas based assemblies and code generation.

Typically used areas and classes/interfaces/services:
* `DeclareServicesSourceGenerator`, `ModuleInitializerSourceGenerator`, `RuntimeElementInfoFactorySourceGenerator`.

## Dependency Injection

Generates code for services registered through one of `[SingletonService]`, `[ScopedService]`, or `[TransientService]` attributes.

## Assembly initializers

Generates code for assembly initializers identified by classes implementing the `IAssemblyInitializer` interface.

## Reflection assembly initializers

Generates code for automatic registration of runtime element information factories into the `RuntimeTypeRegistry.Instance`.

## Other resources

* [Kephas.DependencyInjection.Services](https://www.nuget.org/packages/Kephas.DependencyInjection.Services)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
