// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDispatchingContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDispatchingContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.Resources;
    using Kephas.Services;

    /// <summary>
    /// Interface for dispatching context.
    /// </summary>
    public interface IDispatchingContext : IContext
    {
        /// <summary>
        /// Gets the message broker.
        /// </summary>
        /// <value>
        /// The message broker.
        /// </value>
        IMessageBroker MessageBroker { get; }

        /// <summary>
        /// Gets the brokered message.
        /// </summary>
        /// <value>
        /// The brokered message.
        /// </value>
        IBrokeredMessage BrokeredMessage { get; }

        /// <summary>
        /// Gets or sets the message router which received the message to be dispatched.
        /// </summary>
        /// <value>
        /// The message router.
        /// </value>
        IMessageRouter InputRouter { get; set; }

        /// <summary>
        /// Creates an endpoint for the current application instance.
        /// </summary>
        /// <param name="endpointId">Optional. Identifier for the endpoint.</param>
        /// <param name="scheme">Optional. The address scheme.</param>
        /// <returns>
        /// The new endpoint.
        /// </returns>
        IEndpoint CreateAppInstanceEndpoint(string? endpointId = null, string? scheme = null);
    }

    /// <summary>
    /// Extension methods for <see cref="IDispatchingContext"/>.
    /// </summary>
    public static class DispatchingContextExtensions
    {
        /// <summary>
        /// Sets the router which received the message through the input queue.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="inputRouter">The router.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext InputRouter<TContext>(this TContext context, IMessageRouter inputRouter)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));
            inputRouter = inputRouter ?? throw new System.ArgumentNullException(nameof(inputRouter));

            context.InputRouter = inputRouter;

            return context;
        }

        /// <summary>
        /// Sets the brokered message sender.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="sender">The sender.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext From<TContext>(this TContext context, IEndpoint sender)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));
            sender = sender ?? throw new System.ArgumentNullException(nameof(sender));

            context.BrokeredMessage.Sender = sender;

            return context;
        }

        /// <summary>
        /// Sets the brokered message sender.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="sender">The sender.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext From<TContext>(this TContext context, Endpoint sender)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));
            sender = sender ?? throw new System.ArgumentNullException(nameof(sender));

            context.BrokeredMessage.Sender = sender;

            return context;
        }

        /// <summary>
        /// Sets the brokered message sender.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="sender">The sender.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext From<TContext>(this TContext context, Uri sender)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));
            sender = sender ?? throw new System.ArgumentNullException(nameof(sender));

            context.BrokeredMessage.Sender = new Endpoint(sender);

            return context;
        }

        /// <summary>
        /// Sets the brokered message sender.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="endpointId">The endpoint identification.</param>
        /// <param name="scheme">Optional. The address scheme.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext From<TContext>(this TContext context, string endpointId, string? scheme = null)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));
            Requires.NotNullOrEmpty(endpointId, nameof(endpointId));

            context.BrokeredMessage.Sender = context.CreateAppInstanceEndpoint(endpointId, scheme);

            return context;
        }

        /// <summary>
        /// Sets the brokered message recipients.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="recipients">The recipients.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext To<TContext>(this TContext context, params IEndpoint[] recipients)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.BrokeredMessage.Recipients = recipients;

            return context;
        }

        /// <summary>
        /// Sets the brokered message recipients.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="recipients">The recipients.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext To<TContext>(this TContext context, params Endpoint[] recipients)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.BrokeredMessage.Recipients = recipients;

            return context;
        }

        /// <summary>
        /// Sets the brokered message recipients.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="recipients">The recipients.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext To<TContext>(this TContext context, IEnumerable<IEndpoint> recipients)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.BrokeredMessage.Recipients = recipients;

            return context;
        }

        /// <summary>
        /// Sets the brokered message recipients.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="recipients">The recipients.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext To<TContext>(this TContext context, params Uri[] recipients)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.BrokeredMessage.Recipients = recipients.Select(r => new Endpoint(r)).ToList();

            return context;
        }

        /// <summary>
        /// Sets the brokered message recipients.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="recipients">The recipients.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext To<TContext>(this TContext context, IEnumerable<Uri> recipients)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.BrokeredMessage.Recipients = recipients.Select(r => new Endpoint(r)).ToList();

            return context;
        }

        /// <summary>
        /// Sets the brokered message recipients.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="recipients">The recipients.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext To<TContext>(this TContext context, params string[] recipients)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.BrokeredMessage.Recipients = recipients.Select(r => new Endpoint(new Uri(r))).ToList();

            return context;
        }

        /// <summary>
        /// Sets the brokered message recipients.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="recipients">The recipients.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext To<TContext>(this TContext context, IEnumerable<string> recipients)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.BrokeredMessage.Recipients = recipients.Select(r => new Endpoint(new Uri(r))).ToList();

            return context;
        }

        /// <summary>
        /// Makes the communication one way.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext OneWay<TContext>(this TContext context)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.BrokeredMessage.IsOneWay = true;

            return context;
        }

        /// <summary>
        /// Sets the value of the indicated custom property.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the timeout is negative.</exception>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext Property<TContext>(this TContext context, string name, object value)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));
            name = name ?? throw new System.ArgumentNullException(nameof(name));

            context.BrokeredMessage[name] = value;

            return context;
        }

        /// <summary>
        /// Sets the timeout when waiting for an answer.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the timeout is negative.</exception>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext Timeout<TContext>(this TContext context, TimeSpan timeout)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));
            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentException(Strings.DispatchingContext_NonNegativeTimeout_Exception.FormatWith(timeout), nameof(timeout));
            }

            context.BrokeredMessage.Timeout = timeout;

            return context;
        }

        /// <summary>
        /// Sets the brokered message priority.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="priority">The priority.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext Priority<TContext>(this TContext context, Priority priority)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.BrokeredMessage.Priority = priority;

            return context;
        }

        /// <summary>
        /// Makes the contained message a reply to the provided message.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="message">The message to reply to.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext ReplyTo<TContext>(this TContext context, IBrokeredMessage message)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));
            message = message ?? throw new ArgumentNullException(nameof(message));

            context.ReplyTo(message.Id, message.Sender, message.Trace);
            context.BrokeredMessage.BearerToken = message.BearerToken;
            context.BrokeredMessage.Priority = message.Priority;

            return context;
        }

        /// <summary>
        /// Makes the contained message a reply to the provided message.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="messageId">Identifier for the message.</param>
        /// <param name="sender">Optional. The sender.</param>
        /// <param name="trace">Optional. The trace.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext ReplyTo<TContext>(this TContext context, string messageId, IEndpoint? sender = null, string? trace = null)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));
            Requires.NotNullOrEmpty(messageId, nameof(messageId));

            context.BrokeredMessage.ReplyToMessageId = messageId;
            context.BrokeredMessage.TraceReply(trace, null);
            if (sender != null)
            {
                context.BrokeredMessage.Recipients = new[] { sender };
            }

            context.BrokeredMessage.IsOneWay = true;

            return context;
        }

        /// <summary>
        /// Sets the content message.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext Content<TContext>(this TContext context, IMessage message)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.BrokeredMessage.Content = message;
            if (message is IEvent)
            {
                context.BrokeredMessage.IsOneWay = true;
            }

            return context;
        }

        /// <summary>
        /// Sets the content message.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext ContentMessage<TContext>(this TContext context, object message)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            return context.Content(message.ToMessage());
        }

        /// <summary>
        /// Sets the content message to an event.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The dispatching context.</param>
        /// <param name="event">The event.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext ContentEvent<TContext>(this TContext context, object @event)
            where TContext : class, IDispatchingContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            return context.Content(@event.ToEvent());
        }
    }
}
