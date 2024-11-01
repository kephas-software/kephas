# Serialization (JSON)

## Introduction
Provides common infrastructure for JSON serialization.

## `JsonHelper`: serialization extensions

```csharp
// serialize object using default options.
var serialized = obj.Serialize();

// serialize object using custom options.
var serialized = obj.Serialize(options => options.Format = true);

// serialize object using default options.
var object = str.Deserialize<Result>();
```

## `IJsonService`
This service is typically used within the dependency injection context.
Its registration is automatically done when referencing the package, by the `SerializationServiceRegistration`.

```csharp
// consume service
var serializer = serviceProvider.GetRequiredService<IJsonService>();
var serialized = serializer.Serialize(obj);
var object = serializer.Deserialize<Result>(str);
```

### Extensibility
To add support for custom converters, make sure to register them into the service collection as `JsonConverter`.
They will be then injected into the settings.

```csharp

public class MyCustomJsonConverter : JsonConverter<MyClass>
{
   // ...
}

// register the service
services.AddTransient<JsonConverter, MyCustomJsonConverter, ServiceMetadata>();

// consume service
var serializer = serviceProvider.GetRequiredService<IJsonService>();
var serialized = serializer.Serialize(new MyClass());   // converter should be called
var object = serializer.Deserialize<MyClass>(str);      // converter should be called       
```

## Other resources

* [Kephas.DependencyInjection](https://www.nuget.org/packages/Kephas.DependencyInjection)
* [Kephas.Analyzers](https://www.nuget.org/packages/Kephas.Analyzers)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.

