# Abstractions

## Introduction
This package is the foundation for all the other Kephas packages. It consists of multiple areas, which are described below.

Typically used areas and classes/interfaces/services:
* Application: ``IAppRuntime``, ``AppIdentity``, ``IAppArgs``, ``AppArgs``.
* Commands: ```CommandInfo```, ```IArgs```, ```Args```.
* Logging: ```ILogManager```, ```ILogger```.
* Reflection: ```IPrototype```.
* Serialization: ```ExcludeFromSerializationAttribute```.
* Threading.Tasks: ```PreserveThreadContext``` extension method.

## Dynamic objects

Sometimes it is very useful for objects to provide dynamic behavior, so that, on the fly, new properties may be added to them without recompilation, just like in the dynamic languages like JavaScript or Python.
Their properties ca be accessed:
* directly, with the API defined by their class.
* by the means of the dynamic keyword.
* by their name, just like a dictionary.

### The `IDynamic` interface
Objects supporting settings or getting values by the mean of a string key implement the `IDynamic` interface.

It provides one single indexer:
* `this[key: string]: object`. By this mean, the object may be used like a dictionary.

### `Expando`
A ready-to-use expando class is `Expando`. Upon initialization, a flag controls how the internal dictionary is used: thread safe or not. Depending on it, the inner dictionary is set to a `ConcurrentDictionary` or `Dictionary`.

#### Examples

* Plain expando

```C#
    dynamic expando = new Expando();

    expando.Property = "value";
    Assert.AreEqual("value", expando.Property);
```

* Expando over a dictionary

```C#
    var dictionary = new Dictionary<string, object>();
    dynamic expando = dictionary.ToDynamic();

    expando.Property = "value";
    Assert.AreEqual("value", dictionary["Property"]);
```

* Access over API, dynamic, or indexer

```C#
    public class Person : Expando
    {
        public int Age { get; set; }
    }

    //...
    
    var person = new Person();
    person.Age = 30;     // the age is set through the class API

    dynamic dynPerson = person;
    dynPerson.Age = 23;  // the age is set through the dynamic features
    Assert.AreEqual(23, person.Age);

    person["Age"] = 40;
    Assert.AreEqual(40, person.Age);

    dynPerson.IsOld = true;
    Assert.IsTrue(person["IsOld"]);
```

* Expose non-dynamic objects as dynamic

```C#
    public class Contact
    {
        public string Name { get; set; }
    }

    //...
    
    var contact = new Contact();
    dynamic dynContact = contact.ToDynamic();

    dynContact.Name = "John";
    Assert.AreEqual("John", contact.Name);
```

### Usage
Expandos can be successfully used where a dynamic context is useful. Examples:
* Context objects, used to provide information about the current execution context, passed along the execution flow.
* Metadata objects, used to provide meta information about entities and other artifacts.
* Configuration settings.

### Extensibility through ```ExpandoBase```

Kephas provides through `ExpandoBase` a base implementation for expando objects.
Objects inheriting from an `ExpandoBase` can be used either as a standalone object, or can wrap an existing instance/dictionary and expose it as a dynamic object.

#### Dynamic value access.
To get or set a value from/to an `ExpandoBase`, based on the property name or key, the following steps are taken:
* First of all, the implementation tries to identify a property from the inner object, if one is set, by the provided name/key.
* The next try is to identify the property from the expando object itself.
* Lastly, if still a property by the provided name cannot be found, the inner dictionary is searched/updated by the provided key.

#### Conversion to a dictionary
All the values from the expand can be collected in form a dictionary, using the `ToDictionary()` method. The values are added in their overwrite order:
* First of all, all the inner dictionary entries are added.
* Then, the property values in the current expand object are added or existing values overwritten.
* Lastly, the values from the inner object are added or existing values overwritten.

#### Notes to inheritors
* `TryGetValue(key: string, out value: object): boolean`: Method used to get a value from the expando based on a property name or dictionary key. Override if a logic other than the default presented one should be used.
* `TrySetValue(key: string, value: object): boolean`: Method used to set a value from the expando based on a property name or dictionary key. Override if a logic other than the default presented one should be used.

#### Example

```csharp
    /// <summary>
    /// Expando class for evaluating the internal values on demand, based on a value resolver function.
    /// </summary>
    public class LazyExpando : ExpandoBase<object?>
    {
        private readonly IDictionary<string, object> lockDictionary = new Dictionary<string, object>();
        private readonly IDictionary<string, object?> innerDictionary;

        public LazyExpando(Func<string, object?>? valueResolver = null)
            : this(new Dictionary<string, object?>(), valueResolver)
        {
        }

        public LazyExpando(IDictionary<string, object?> dictionary, Func<string, object?>? valueResolver = null)
            : base(dictionary ?? throw new ArgumentNullException(nameof(dictionary)))
        {
            this.innerDictionary = dictionary;
            this.ValueResolver = valueResolver;
        }

        protected Func<string, object?>? ValueResolver { get; set; }

        protected override bool TryGetValue(string key, out object? value)
        {
            if (base.TryGetValue(key, out value))
            {
                return true;
            }

            var valueResolver = this.ValueResolver;
            if (valueResolver != null)
            {
                if (this.lockDictionary.ContainsKey(key))
                {
                    return this.HandleCircularDependency(key, out value);
                }

                try
                {
                    this.lockDictionary[key] = true;
                    value = valueResolver(key);
                    this.innerDictionary[key] = value;
                }
                finally
                {
                    this.lockDictionary.Remove(key);
                }

                return true;
            }

            return false;
        }

        protected virtual bool HandleCircularDependency(string key, out object? value)
        {
            throw new CircularDependencyException($"Circular dependency among values involving '{key}'.");
        }
    }
```

```csharp
dynamic expando = new LazyExpando(name => name == "Name" ? "John Doe" : "unset");
Assert.Equal("John Doe", expando.Name);
Assert.Equal("unset", expando.Age);
```

### Extensibility through ```IExpandoMixin```

Alternatively, the ```IExpandoMixin``` interface can be implemented. Basically, in this case, only the ```IExpandoMixin.InnerDictionary``` needs to be implemented.

#### Example

```csharp
    public class Expandable : IExpandoMixin
    {
        private IDictionary<string, object?> inner = new Dictionary<string, object?>();

        IDictionary<string, object?> IExpandoMixin.InnerDictionary => this.inner;

        public string? Name { get; set; }
    }
```

```csharp
dynamic expandable = new Expandable();
expandable.Name = "John Doe"; // value is set into the class property
expandable.Age = 34; // value is set into the inner dictionary
```

## Assembly initialization

Tme [module initializer feature](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/module-initializers?source=recommendations) is supported starting with .NET 5 and C# 9.
To leverage it, Kephas introduced the `IAssemblyInitializer` interface and the `AssemblyInitializerAttribute`, which can be used to perform assembly initialization upon loading.
The [Kephas.Analyzers](https://www.nuget.org/packages/Kephas.Analyzers) package is aware of these and generates the code according to the following rules:
* A class is generated with a single `InitializeModule` method, annotated appropriately.
```csharp
    internal static class ModuleInitializer_My_Assembly
    {
        [ModuleInitializer]
        internal static void InitializeModule()
        {
            // ...
        }
    }
```
* All classes implementing the `IAssemblyInitializer` interface are instantiated and their `Initialize` method is called.
  * It is by-design to have instance classes instead of static classes.
* All `AssemblyInitializerAttribute`s are collected from the executing assembly, their `InitializerTypes` iterated, instantiated, and their `Initialize` method called.
  * Typically, the `AssemblyInitializerAttribute` is used by source generators to provide at runtime assembly initializers not available at code generation time, like those defined by code generators.

> Recommendation: Prefer using statically invoked assembly initializers instead by using the `AssemblyInitializerAttribute`.
> This is solely for increasing loading time performance through avoiding reflection.

> Caution: do not add to an assembly referencing the [Kephas.Analyzers](https://www.nuget.org/packages/Kephas.Analyzers) package
> a `[ModuleInitializer]` annotated method, as it will collide with the one generated.

## Other resources

* [Kephas.Logging](https://www.nuget.org/packages/Kephas.Logging)
* [Kephas.Injection](https://www.nuget.org/packages/Kephas.Injection)
* [Kephas.Reflection](https://www.nuget.org/packages/Kephas.Reflection)
* [Kephas.Analyzers](https://www.nuget.org/packages/Kephas.Analyzers)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.

