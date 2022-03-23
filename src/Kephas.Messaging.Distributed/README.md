# Distributed messaging

The [[message processing|Architecture-of-messaging]] is an in-process feature, which is not _per se_ bad, but in distributed scenarios it is limiting. The distributed messaging fills the gap by providing infrastructure components that can communicate in a such an environment.

Using the messaging infrastructure is a little more elaborate:

```C#
    // ensure the message has been sent
    await messageBroker.DispatchAsync(new RefreshCacheEvent { Key = "Users" }, ctx => ctx.OneWay()).PreserveThreadContext();   
```

This can be achieved in a simpler way, using the provided extension methods:

```C#
    // ensure the message has been sent
    await messageBroker.PublishAsync(new RefreshCacheEvent { Key = "Users" }).PreserveThreadContext();   
```

# The distributed messaging flow

![Distributed Messaging](https://github.com/kephas-software/kephas/blob/master/docs/images/messaging/Messaging.Inter-Process.png)

The participants in the distributed message flow are:

* The _brokered message_: This is the message being sent through the underlying infrastructure to the processors.
* The _message broker_: This is the in-process component dispatching the messages by the means of the underlying infrastructure.
* The _brokered message handler_: This is the [[message handler|Architecture-of-messaging#message-handlers]] on the other end, receiving the brokered message and ensuring that it is processed and, if requested, a proper response is sent back.
* Infrastructure dependent components which:
  * send/queue messages.
  * read the messages and forward them to the in-process [[message processor|Architecture-of-messaging#the-message-processor]] .

# Brokered message

A brokered message (`IBrokeredMessage`) is a specialization of a [[message|Architecture-of-messaging#messages]] carrying with it the original message to be processed and some distributed environment information. It can be regarded as an "envelope" transporting the actual message.

* _Content_: the message to be actually processed.
* _Sender_: contains information about the message sender. For some broadcasts, it can be used to not forward the message to the original sender.
* _Channel_: contains the channel to use. If not provided, the default channel will be used.
* _IsOneWay_: indicates whether the sender awaits an answer to the message, or is just a _fire and forget_ scenario.
* _Timeout_: the timeout for requests awaiting for a response.

# The message broker

This is a [[singleton application service|Application-Services#shared-scope-shared-or-instance-based-services]] dispatching messages through a channel to registered processors.

The `IMessageBroker` service contract has the following methods:

* `DispatchAsync(object message, [optionsConfig: Action<IDispatchingContext>], [cancellationToken: CancellationToken]): Task<IMessage>`: Dispatches the brokered message asynchronously.
  * Depending on the _IsOneWay_ setting in the brokered message, it waits for a response or not.

## `InProcessMessageBroker`

The in-process message broker is the Kephas implementation for a message broker dispatching the message to the in-process [[message processor|Architecture-of-messaging#the-message-processor]]. It has the [[lowest override priority|Application-Services#override-priority]].

> This is the default message broker used when no custom one has been defined. It is recommended to use real-world implementations, using established message queuing infrastructure.

# The brokered message handler

Brokered messages are nothing more than [[message|Architecture-of-messaging#messages]] requiring proper handling. The brokered message handler is the default implementation forwarding the contained message to the in-process [[message processor|Architecture-of-messaging#the-message-processor]].