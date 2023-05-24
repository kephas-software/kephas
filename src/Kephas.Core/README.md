# Core

## Introduction
This package provides common abstractions for Kephas components.

Typically used areas and classes/interfaces/services:
* Reflection: ```IPrototype```.
* Model: ```ExcludeFromModelAttribute```.
* Serialization: ```ExcludeFromSerializationAttribute```.

## Adapters

The `IAdapter` interface is used to implement adapters.

```csharp
public class ThirdPartyComponentAdapter : IAdapter<ThirdPartyComponent>
{
    public ThirdPartyComponentAdapter(ThirdPartyComponent component)
    {
        this.component = component;
    }
    
    ThirdPartyComponent IAdapter<ThirdPartyComponent>.Of => this.component;
}
```

## Other resources

* [Kephas.Dynamic](https://www.nuget.org/packages/Kephas.Dynamic)
* [Kephas.Exceptions](https://www.nuget.org/packages/Kephas.Exceptions)
* [Kephas.Logging.Abstractions](https://www.nuget.org/packages/Kephas.Logging.Abstractions)
* [Kephas.Services.Abstractions](https://www.nuget.org/packages/Kephas.Services.Abstractions)
* [Kephas.Services](https://www.nuget.org/packages/Kephas.Services)
* [Kephas.Reflection](https://www.nuget.org/packages/Kephas.Reflection)
* [Kephas.Analyzers](https://www.nuget.org/packages/Kephas.Analyzers)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.

