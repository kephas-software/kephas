# Web Exception Handling

This NuGet package Provides support for exception handling in web projects.

## The exception result factory
The `IExceptionResultFactory` is a singleton service which has the responsibility of creating an `ExceptionResult`
based on a given exception. Typically, it is used by REST endpoints to provide an exception response to the callers.

The default implementation aggregates multiple `IExceptionResultProvider` services.
Each of these providers is responsible for handling a specific exception type, and the first one
being able to handle the exception will be chosen to handle the transformation.

#### Example

```csharp
var resultProvider = serviceProvider.GetRequiredService<IExceptionResultProvider>();
var result = resultProvider.GetExceptionResult(new ExceptionContext(new InvalidOperationException("something bad happened.")));

Assert.Equal("something bad happened.", result.Message);
```

## Exception providers
* `AppExceptionResultProvider`: handles exceptions implementing `AppException`.
* `NotImplementedExceptionResultProvider`: handles exceptions of type `NotImplementedException`.
* `UnauthorizedAccessExceptionResultProvider`: handles exceptions of type `UnauthorizedAccessException`.
* `GenericExceptionResultProvider`: handles all exceptions, but has the lowest priority.

### Custom exception providers
To add support for custom exceptions, follow these steps:
* Add a class implementing `IExceptionResultProvider`.
* Optional. Specify a `[Priority]` in case a specific exception type needs to be overridden.

## Exception handling in web application

Simply reference the `Kephas.Web.Exceptions` package. It will automatically use `ErrorHandlingServiceRegistration`
to add the exception handling services.

```csharp
var builder = WebApplication.CreateBuilder(args);
//...
var app = builder.Build();
//...
app.UseExceptionHandler();
```

By default, the infrastructure is set up to use the `IRequestExceptionHandler` service to handle the exceptions.
The default implementation (`RequestExceptionHandler`) makes sure that the errors are properly logged and that an
appropriate message and status code are sent back to the client in form of an `ExceptionResult`.

If needed, the default service may be replaced with a custom one - just make sure to register it with a higher priority.

### Extensibility: handling custom exception types

To handle custom exception types, follow the steps:
* Define an exception result provider service. Make sure to inherit from the `IExceptionResultProvider<TException>` interface.
* Optional: provide a `[Priority(<priority>)]` attribute to indicate a higher or a lower priority in selecting this service.

If not set by the provider, the returned `ExceptionResult` will have:
* the `StatusCode` set to `InternalServerError`.
* the `Message` set to `exception.Message`.
* the `InnerException` set to `exceptionResultFactory.GetExceptionResult(exception.InnerException)`.
* the `Severity` set to `Error`.

> Note: when implementing the TryGetExceptionResult method, returning a `null` value indicates that the request could not be handled.

#### Example

```csharp
internal class InvalidOperationExceptionResultProvider : IExceptionResultProvider<InvalidOperationException>
{
    public ExceptionResult? TryGetExceptionResult(InvalidOperationException exception, IExceptionContext exceptionContext) =>
        new()
        {
            StatusCode = (int)HttpStatusCode.BadRequest,
            Message = exception.Mesage,
            Severity = SeverityLevel.Critical,
        };
}
```

## Other resources

* Please check the [release notes](release-notes.md) file in this package.

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
