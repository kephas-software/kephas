# Messaging (Contracts)

This package provides the contracts for messaging.

## Messages

The message is the content of communication. A message:
* typically implements `IMessage`, but it is only recommended. This is only a marker interface so that message types may be identified and used through reflection by other components.
  * for example, a discovery system may identify the messages and provide documentation for them.
  * messages not implementing `IMessage` are anonymous and cannot be discovered through reflection.
* is serializable. This is somehow natural, as the communication may happen inter-processes and even across machines.

### Strongly typed and weakly typed messages

The message type plays an important role, as it is used to filter message handlers (more about this in the [message handlers](#message-handlers) section.
However, there are so called _weakly typed_ messages, which hold actually a category of messages that for some reason
cannot be _strongly typed_, typically used in distributed scenarios and coming from external sources.
These _weakly typed_ messages will provide a name, used for discriminating the message handlers, retrieved as:
* the _MessageName_ property of the message class -or-
* the dynamic _MessageName_ property in the case of a dynamic message object.

## Events

Events are a special case of messages targeting the _Publisher/Subscriber_ pattern. An event:
* implements `IEvent`. This is also a marker interface inheriting from `IMessage`.

## Securing messages

Messages may be secured by decorating them with the `[RequiresPermission]` attribute.

#### Example:

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
