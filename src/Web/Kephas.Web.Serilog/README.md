# Web logging with Serilog

Provides unified logging functionalities to ASP.NET Core projects using the ```Serilog``` library.

## Required Steps

* Configure the logging in the ```appsettings.json``` configuration file.
* Configure the logging in code.
* Inject the logger in the service requiring logging.

## Configure the logging in ```appsettings.json```

Ensure that you have:
* The ```Serilog``` section from the ```appsettings.json``` configuration file.
    * The ```MinimumLevel``` and the ```path``` must be build variable references.

```json
{
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "JsonFormattedFile",
        "Args": {
          "path": "C:\\Temp\\Logs\\myapp.json"
        }
      },
      {
        "Name": "Console"
      }
    ]
  }
}
```

Notes on this example:
* There is no need to specify the following enrichers: LogContext, Thread, Process, Environment, ExceptionDetails, if using the ```UseLogging``` extension method (check the next sections in this document), as they are included by default.

## Configure the logging in code
* Make sure that this nuget package (`Kephas.Web.Serilog`) is added to project package references.
* Add the following code to your application startup, typically `Program.cs`.

```csharp
var hostBuilder = Host.CreateDefaultBuilder(args);
hostBuilder.UseWebLogging((context, serviceProvider, loggingConfiguration) => {
    // add additional configuration code.
    // by default, the following are performed:
    // * the logger configuration is read from the context.Configuration
    // * the LogContext, ThreadId, ProcessId, MachineName, and ExceptionDetails enrichers are added.
    // * the SelfLog is redirected to Console.Error.
    // * the additional configuration is called. 
});
```

If no additional configuration is needed simply invoke the UseLogging() method without any parameters.
```csharp
hostBuilder.UseWebLogging();
```

## Inject the logger in the service requiring logging

When a class requires the logger, simply add it as a parameter in the constructor of the class and the DI framework
will inject the logger in the class.

```csharp
public MyService(ILogger<MyService> logger)
{
    this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

# Other resources

* Please check the [release notes](release-notes.md) file in this package.

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
