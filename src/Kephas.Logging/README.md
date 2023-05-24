# Logging

## Introduction

Provides the infrastructure for logging and integration with the dependency injection.

Typically used areas and classes/interfaces/services:
* ``ILogger``, ``Loggable``, ``TypedLogger``.

The logging abstractions provide implementation agnostic logging contracts.
The loggers are available through [Injection](https://www.nuget.org/packages/Kephas.Services).

Packages providing specific logging implementations:
* [Kephas.Logging.Serilog](https://www.nuget.org/packages/Kephas.Logging.Serilog)
* [Kephas.Logging.NLog](https://www.nuget.org/packages/Kephas.Logging.NLog)
* [Kephas.Logging.Log4Net](https://www.nuget.org/packages/Kephas.Logging.Log4Net)

### The ```ILogManager``` singleton service
The log manager is a service registered in the [app services](https://www.nuget.org/packages/Kephas.Services#ambient-services). It provides the following method:
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
* Do not use directly specific loggers, like [Serilog](https://www.nuget.org/packages/Serilog) or [NLog](https://www.nuget.org/packages/NLog), instead prefer the ``ILogger`` interface. Reason: someday the circumstances may dictate to change the logging framework, and it will be a lot easier to change only the logger implementation compared to all logger specific code.

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
These classes should use the globally defined ``ILogManager`` through ``LoggingHelper.DefaultLogManager``.

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

> Caution: Make sure that at the time of calling ```DefaultLogManager``` it is properly initialized with the desired log manager, otherwise a logger discarding log messages will be provided.

## Other resources

* [Kephas.Core](https://www.nuget.org/packages/Kephas.Core)
* [Kephas.Services](https://www.nuget.org/packages/Kephas.Services)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
