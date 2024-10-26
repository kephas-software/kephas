# Extensions (Mediator)

## Introduction
Provides support for the mediator pattern using the [MediatR](https://www.nuget.org/packages/Kephas.MediatR) library.

## Introduction

The [MediatR](https://www.nuget.org/packages/MediatR) library provides the foundation for request processing and notifications.
However, these extensions enhances it with the following features:

* Polymorphic request handling.
* Handling of messages which do not implement the `IBaseRequest` interface.
* Message dispatching infrastructure.

## Dependency injection registration

#### Example

```csharp
services
    .AddApplicationService(configuration);
```

## Polymorphic request processing

By default, the [MediatR](https://www.nuget.org/packages/MediatR) infrastructure uses an exact match for the request type when identifying the handler.
However, this may introduce limitations when trying to handle in a generic way request hierarchies.

* Inherit one of the base classes:
  * `RequestHandlerBase<TRequest, TResponse>`
  * `RequestHandlerBase<TRequest>`
  * `StreamRequestHandlerBase<TRequest, TResponse>`

```csharp
public interface ICommand : IRequest<object?> {}
public class PingCommand : ICommand {}
public class QuitCommand : ICommand {}

public class CommandHandler : RequestHandlerBase<ICommand>
{
    public override Task Handle(ICommand request, CancellationToken cancellationToken)
    {
        // handle here all commands, including PingCommand and QuitCommand
    }
}
```

## Polymorphic pipeline behaviors

By default, the mediator pipeline behaviors must match the request type. To overcome such limitations, inherit the `PipelineBehaviorBase<TRequest, TResponse>` class.

```csharp
public interface ICommand : IRequest<object?> {}
public class PingCommand : ICommand {}
public class QuitCommand : ICommand {}

public class CommandLoggingPipelineBehavior : PipelineBehaviorBase<ICommand, object?>
{
    // handle here all commands, including PingCommand and QuitCommand
    public Task<object?> Handle(ICommand request, RequestHandlerDelegate<object?> next, CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var correlationId = Guid.NewGuid();
        this.Logger.Trace("Starting invocation of {RequestType}/{CorrelationId:N}...", request.GetType(), correlationId);
        var result = await next();
        stopwatch.Stop();
        this.Logger.Trace("Invocation of {RequestType}/{CorrelationId:N} completed in {CompletionTime}.", request.GetType(), correlationId, stopwatch.Elapsed);

        return result;
    }
}
```

## Other resources

* [Kephas.Exceptions](https://www.nuget.org/packages/Kephas.Exceptions)
* [Kephas.Extensions.DependencyInjection](https://www.nuget.org/packages/Kephas.Extensions.DependencyInjection)
* [Kephas.DependencyInjection](https://www.nuget.org/packages/Kephas.DependencyInjection)
* [Kephas.Analyzers](https://www.nuget.org/packages/Kephas.Analyzers)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.

