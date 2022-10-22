# Messaging

Application components and services are just pieces in a big puzzle, requiring in most cases to interact and communicate. The simplest interaction is when inside a component the counterpart is identified, either directly or by the means of [[dependency injection|Composition and Dependency Injection]], and the required API method is called. This implies that the consumer knows which provider handles the message and that both components, consumer and provider, live in the same process.

The messaging infrastructure Kephas provides addresses these key issues:
* The communication may be performed among loosely coupled components. The components communicate through the means of a _message_, not knowing about each other, not being necessarily in the same process.
* Two kinds of patterns are supported by default:
  * _Request/Response_: The requester send a message to be processed and awaits for the response.
  * _Publisher/Subscriber_: The publisher emits an event for which subscribers are notified when it occurs.
* The infrastructure may be extended to support other types of patterns, as needed.

To anticipate a little bit, using the messaging infrastructure is as simple as that:

```C#
    var message = new PingMessage();
    var pingBack = await messageProcessor.ProcessAsync(message).PreserveThreadContext();
    var serverTime = pingBack.ServerTime;
```

So, there is no information whatsoever about who processes the message, only the message processor taking the responsibility of carrying this to a good end.

# Messages

The message is the content of communication. A message:
* typically implements `IMessage`, but it is only recommended. This is only a marker interface so that message types may be identified and used through reflection by other components.
  * for example, a discovery system may identify the messages and provide documentation for them.
  * messages not implementing `IMessage` are anonymous and cannot be discovered through reflection.
* is serializable. This is somehow natural, as the communication may happen inter-processes and even across machines.

## Strongly typed and weakly typed messages

The message type plays an important role, as it is used to filter message handlers (more about this in the [message handlers](#message-handlers) section.
However, there are so called _weakly typed_ messages, which hold actually a category of messages that for some reason
cannot be _strongly typed_, typically used in distributed scenarios and coming from external sources.
These _weakly typed_ messages will provide a name, used for discriminating the message handlers, retrieved as:
* the _MessageName_ property of the message class -or-
* the dynamic _MessageName_ property in the case of a dynamic message object.

## Events

Events are a special case of messages targeting the _Publisher/Subscriber_ pattern. An event:
* implements `IEvent`. This is also a marker interface inheriting from `IMessage`.

# Message handlers

A message handler is an application service processing a message of a given type and, optionally, name.
* implements the `IMessageHandler<TMessage>` application service contract.
* provides the `ProcessAsync` method where the message handling takes place.
  * `ProcessAsync(message: TMessage, context: IMessageProcessingContext, token: CancellationToken): Task<IMessage>`
* (optionally) is annotated with the `[MessageName]` attribute, to indicate the name of the handled message in case of _weakly typed_ messages.

Typically, a message handler specializes the `MessageHandlerBase<TMessage, TResponseMessage>` base class, which takes care of basic message type checks and provides an overridable typed `ProcessAsync` method.

> Note: message handlers are **NOT** singleton application services. This is by design, so that any resources hold during the message processing can be disposed freely at the end. Therefore they can be safely implemented as _statefull_.

> It is the sole responsibility of the message processor to make sure that the handler receives the appropriate messages which it is registered for. The message handler assumes it receives only messages of the declared type and name.

# The message processor

The service taking care of message processing is the message processor. It is a [singleton application service](https://www.nuget.org/packages/Kephas.Services#configuring-the-application-service-contracts) which is in charge of selecting the appropriate handler (or handlers) for a particular message, ensuring that that (or those) handlers handle the message, and returning a response. It provides a single method:

* `ProcessAsync(message: IMessage, [context: IMessageProcessingContext], [token: CancellationToken]): Task<IMessage>`
  * _message_: the message to process.
  * _context_: contextual information related to the particular call. If no processing context is provided, a default one will be created. The processor ensures that the context contains the processor itself, the original message to be processed, and the handler which is at that moment handling the message. This information may be useful in the [[processing behaviors|Architecture-of-messaging#Processing behaviors]], as described in the following sections.

Kephas provides a default implementation, see the [default message processor](https://www.nuget.org/packages/Kephas.Messaging#the-defaultmessageprocessor) for more information about it.

# Handler selectors

The typical message bus implementations do not filter the message handlers, leaving them the responsibility of handling or not handling a particular message. In Kephas, this responsibility is delegated to the handler selectors, which decide if and how the handlers are filtered before letting them handle a message. They provide two methods for interaction with the message processor:

* `CanHandle(messageType: Type, messageName: string): boolean`: Indicates whether the selector can handle the indicated message type. This is the method by which the selectors are requested to indicate whether they are in charge of providing the handlers for a specific message type and name.
* `GetHandlersFactory(messageType: Type, messageName: string): Func<IEnumerable<IMessageHandler>>`: Gets a factory which retrieves the components handling messages of the given type.

## `EventMessageHandlerSelector`

This is a handler selector for [events](https://www.nuget.org/packages/Kephas.Messaging#events). It returns all the handlers processing the specific event type and name, ordered by their processing priority.

> Note: it has the `Low` processing priority, so that custom code can modify easily the strategy of selecting event handlers.

## `DefaultMessageHandlerSelector`

This is the fallback handler selector for generic messages. It returns a single handler processing the specific message type and name, in the order of override and processing priority. If two or more handlers have the same override and processing priority, an `AmbiguousMetchException` occurs.

> Note: this selector has the lowest processing priority, being the fallback for messages not handled by any other selector.

# Processing behaviors

The processing of a message may be intercepted be message processing behaviors, before and after the actual handler invocation. They can do various things, like auditing, authorization checking, or whatever other functionality may be needed. Behaviors are [[singleton application services|Application-Services#shared-scope-shared-or-instance-based-services]], so they should no store any processing information, but use exclusively the processing context for such scenarios. They implement the contract `IMessageProcessingBehavior` which provides two methods:

* `BeforeProcessAsync(context: IMessageProcessingContext, token: CancellationToken): Task`: Interception called before invoking the handler to process the message.
* `AfterProcessAsync(context: IMessageProcessingContext, token: CancellationToken): Task`: Interception called after invoking the handler to process the message.

# In-process messaging flow

![In-Process Messaging](https://github.com/kephas-software/kephas/blob/master/docs/images/messaging/Messaging.In-Process.png)

# The `DefaultMessageProcessor`

Kephas provides the `DefaultMessageProcessor` class, a [low override priority](https://www.nuget.org/packages/Kephas.Services#override-priority) message processor. It provides a basic, overridable functionality for processing messages, which should be enough for most cases.

* It aggregates _message handler selectors_ and calls them to provide a list of message handlers to process a particular message, in their [processing priority](https://www.nuget.org/packages/Kephas.Services#processing-priority) order.
* It aggregates _processing filters_ and calls them before and after each handler's `ProcessAsync` call.

> Note that the message processor is an in-process service. However, if the handlers themselves go beyond the process boundary it is their sole responsibility and, at the same time, freedom.

The flow of the `ProcessAsync` implementation is as follows:
* Resolve the processing filters, ordered by their processing priority.
* Resolve the handlers, as provided by the handler selectors called in their processing priority order.
  * Question the selectors in the indicated order whether they can provide handlers for the message.
  * The first selector answering with `true` is delegated to provide the handlers, the rest are ignored.
* Call each handler to process the message, **awaiting for their response**.
  * Before calling the handler, the ordered filters are invoked.
  * After calling the handler, the filters are invoked in the inverse order.
* Returns the response of the last called handler.

> Note: if the handler throws an exception, the processing filters are called in the _after_ method. with the exception information set in the context. However, if a filter throws an exception during the processing, it interrupts the flow and the exception is thrown to the caller. This is by design, so that, for example, authorization filters are able to interrupt the processing flow.

# Securing the messaging infrastructure

The responsibility of the message processor during execution is to ensure that the provided message gets handled by one or more handlers, and that the behaviors are properly called; however it remains agnostic to the semantics of the message itself. The authorization check is ensured by behaviors:

* `EnsureAuthorizedMessageProcessingBehavior` takes the job of ensuring that the handling of the message is authorized.
  * Retrieves the required permissions as specified at message level.
  * Identifies the authorization scope by invoking the [authorization scope service](https://www.nuget.org/packages/Kephas.Security).
  * Invokes the [authorization service](https://www.nuget.org/packages/Kephas.Security) to authorize the scope for the required permissions.

## Securing messages

Messages may be secured by decorating them with the `[RequiresPermission]` attribute.

Example:

```C#
    /// <summary>
    /// Message for importing a hierarchy node.
    /// </summary>
    [RequiresPermission(typeof(IExportImportPermission))]
    public class ImportHierarchyMessage : EntityActionMessage
    {
        // ...
    }
```
