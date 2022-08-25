# Abstractions

## Introduction
This package is the foundation for all the other Kephas packages. It consists of multiple areas, which are described below.

* [Logging](#logging)
* [Dynamic objects](#dynamic-objects)

Check the following packages for more information:
* [Kephas.Injection](https://www.nuget.org/packages/Kephas.Injection)

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
    dynamic expando = new Expando(dictionary);

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
    dynamic dynContact = new Expando(contact);

    dynContact.Name = "John";
    Assert.AreEqual("John", contact.Name);
```

### Usage
Expandos can be successfully used where a dynamic context is useful. Examples:
* Context objects, used to provide information about the current execution context, passed along the execution flow.
* Metadata objects, used to provide meta information about entities and other artifacts.
* Configuration settings.

## Other resources

* [Kephas.Reflection](https://www.nuget.org/packages/Kephas.Reflection)


    Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
