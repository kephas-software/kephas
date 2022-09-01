# Abstractions

## Introduction
This package is the foundation for all the other Kephas packages. It consists of multiple areas, which are described below.

* [Logging](#logging)
* [Dynamic objects](#dynamic-objects)

Check the following packages for more information:
* [Kephas.Injection](https://www.nuget.org/packages/Kephas.Injection)

Typically used areas and classes/interfaces/services:
* Commands: ```CommandInfo```, ```IArgs```, ```Args```.
* Logging: ```ILogManager```, ```ILogger```.
* Reflection: ```IPrototype```.
* Serialization: ```ExcludeFromSerializationAttribute```.
* Threading.Tasks: ```PreserveThreadContext``` extension method.
* Versioning: ```SemanticVersion```.

## Logging
The logging abstractions provide implementation agnostic logging contracts.
The loggers are available through [Injection](https://www.nuget.org/packages/Kephas.Injection).

Packages providing specific logging implementations:
* [Kephas.Logging.NLog](https://www.nuget.org/packages/Kephas.Logging.NLog)
* [Kephas.Logging.Serilog](https://www.nuget.org/packages/Kephas.Logging.Serilog)
* [Kephas.Logging.Log4Net](https://www.nuget.org/packages/Kephas.Logging.Log4Net)

### The ```ILogManager``` singleton service
The log manager is a service registered in the [ambient services](https://www.nuget.org/packages/Kephas.Injection#ambient-services). It provides the following method:
* `GetLogger(loggerName): ILogger`: retrieves the logger with the provided name.
* `GetLogger<T>(): ILogger` (extension): retrieves the logger having the same name with the generic type's argument full name.

By default, a `NullLoggerManager` will be used, if not overwritten with a more specific implementation (NLog, Serilog, Log4Net, or other).

### The logger
A logger implements the `ILogger` contract, is identified by its name and is created by the log manager. For injection purposes, a generic `ILogger<TService>` service contract is provided to be imported by injectable services.

It provides a single method which logs the provided arguments at the indicated level:
* `Log(level: LogLevel, exception?: Exception, messageFormat: string, params args: object[])`.

> Note to implementors: the exception may be `null`, so be cautious and handle this case too. For example, the ``LoggerExtensions.Log`` extension method passes a `null` exception.

For convenience, however, extension methods are provided to log fatal errors, errors, warnings, information, debug and trace data.

### Consuming logging services
There are two kinds of components which can consume logging services:
* Injectable services.
* Non-injectable services and static classes.

To log anything, each consumer must firstly receive the logger it requires to consume its services by using specific means.
Recommendations:
* Do not use directly specific loggers, like log4net or NLog, instead prefer the ``ILogger`` interface. Reason: someday the circumstances may dictate to change the logging framework, and it will be a lot easier to change only the logger implementation compared to all logger specific code.

#### Injectable services
Injectable services should just import the logger through the constructor, with the type `ILogger<ServiceType>`, where `ServiceType` is the type of the service.
Alternatively, the service can extend ```Loggable``` by providing it, optionally, an ```ILogManager```.

* Example

```C#
    [SingletonAppServiceContract]
    public interface IModelContainer
    {
        //...
    }

    public class ModelContainer : IModelContainer
    {
        public ModelContainer(ILogger<ModelContainer> logger)
        {
            this.Logger = logger;        
        }

        protected ILogger<ModelContainer> Logger { get; }
    }

    // - or - inherit from Loggable, if there are no base class restrictions

    public class ModelContainer : Loggable, IModelContainer
    {
        public ModelContainer(ILogManager? logManager = null)
            : base(logManager)
        {
        }
    }
```

#### Non-injectable services and static classes
These classes should use the globally defined ```ILogManager``` through ```LoggingHelper.DefaultLogManager```.

* Examples

```c#
    public static class ReflectionHelper
    {
        /// <summary>
        /// Logger instance.
        /// </summary>
        private static readonly ILogger Logger = LoggingHelper.DefaultLogManager.GetLogger<ReflectionHelper>();
	
        //...
    }
```

> Caution: Make sure that at the time of calling ```DefaultLogManager``` it is properly initialized with the desired log manager, otherwise a logger logging nothing will be provided.

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
    dynamic expando = dictionary.ToExpando();

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
    dynamic dynContact = contact.ToExpando();

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

## Other resources

* [Kephas.Reflection](https://www.nuget.org/packages/Kephas.Reflection)


> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
