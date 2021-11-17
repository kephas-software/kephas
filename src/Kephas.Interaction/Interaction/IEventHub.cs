// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEventHub.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEventHub interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Interaction
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// Contract for the singleton application service handling in-process event publishing/subscribing.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IEventHub
    {
        /// <summary>
        /// Publishes the event asynchronously to its subscribers.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result containing the operation result.
        /// The encapsulated value is an enumeration of return values from each subscriber.
        /// </returns>
        Task<IOperationResult<IEnumerable<object?>>> PublishAsync(object @event, IContext? context = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes the event asynchronously to its subscribers.
        /// </summary>
        /// <typeparam name="TContext">The context type.</typeparam>
        /// <param name="event">The event.</param>
        /// <param name="options">The options to configure the context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result containing the operation result.
        /// The encapsulated value is an enumeration of return values from each subscriber.
        /// </returns>
        Task<IOperationResult<IEnumerable<object?>>> PublishAsync<TContext>(object @event, Action<TContext> options, CancellationToken cancellationToken = default)
            where TContext : class, IContext;

        /// <summary>
        /// Subscribes to the event(s) matching the criteria.
        /// </summary>
        /// <param name="match">Specifies the match criteria.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        IEventSubscription Subscribe(Func<object, bool> match, Func<object, IContext?, CancellationToken, Task> callback);

        /// <summary>
        /// Subscribes to the event(s) matching the provided type.
        /// </summary>
        /// <param name="typeMatch">Specifies the type match criteria.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        IEventSubscription Subscribe(Type typeMatch, Func<object, IContext?, CancellationToken, Task> callback);
    }

    /// <summary>
    /// Extensions for <see cref="IEventHub"/>.
    /// </summary>
    public static class EventHubExtensions
    {
        /// <summary>
        /// Publishes the event asynchronously to its subscribers.
        /// </summary>
        /// <param name="eventHub">The event hub to act on.</param>
        /// <param name="event">The event.</param>
        /// <param name="options">The options to configure the context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result containing the operation result.
        /// The encapsulated value is an enumeration of return values from each subscriber.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task PublishAsync(this IEventHub eventHub, object @event, Action<IContext> options, CancellationToken cancellationToken = default)
            => eventHub.PublishAsync<Context>(@event, options, cancellationToken);

        /// <summary>
        /// Subscribes to the event(s) matching the provided type.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="eventHub">The event hub to act on.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        public static IEventSubscription Subscribe<TEvent>(this IEventHub eventHub, Func<TEvent, IContext?, CancellationToken, Task> callback)
            where TEvent : class
        {
            eventHub = eventHub ?? throw new ArgumentNullException(nameof(eventHub));
            callback = callback ?? throw new ArgumentNullException(nameof(callback));

            return eventHub.Subscribe(typeof(TEvent), (e, ctx, token) => callback((TEvent)e, ctx, token));
        }

        /// <summary>
        /// Subscribes to the event(s) matching the provided type.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <typeparam name="TResult">The type of the result returned by the event handler.</typeparam>
        /// <param name="eventHub">The event hub to act on.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        public static IEventSubscription Subscribe<TEvent, TResult>(this IEventHub eventHub, Func<TEvent, IContext?, CancellationToken, Task<TResult>> callback)
            where TEvent : class
        {
            eventHub = eventHub ?? throw new ArgumentNullException(nameof(eventHub));
            callback = callback ?? throw new ArgumentNullException(nameof(callback));

            return eventHub.Subscribe(typeof(TEvent), (e, ctx, token) => callback((TEvent)e, ctx, token));
        }

        /// <summary>
        /// Subscribes to the event(s) matching the provided type.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="eventHub">The event hub to act on.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        public static IEventSubscription Subscribe<TEvent>(this IEventHub eventHub, Action<TEvent, IContext?> callback)
            where TEvent : class
        {
            eventHub = eventHub ?? throw new ArgumentNullException(nameof(eventHub));
            callback = callback ?? throw new ArgumentNullException(nameof(callback));

            return eventHub.Subscribe(
                typeof(TEvent),
                (e, ctx, token) =>
                    {
                        callback((TEvent)e, ctx);
                        return Task.CompletedTask;
                    });
        }

        /// <summary>
        /// Subscribes to the event(s) matching the provided type.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <typeparam name="TResult">The type of the result returned by the event handler.</typeparam>
        /// <param name="eventHub">The event hub to act on.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        public static IEventSubscription Subscribe<TEvent, TResult>(this IEventHub eventHub, Func<TEvent, IContext?, TResult> callback)
            where TEvent : class
        {
            eventHub = eventHub ?? throw new ArgumentNullException(nameof(eventHub));
            callback = callback ?? throw new ArgumentNullException(nameof(callback));

            return eventHub.Subscribe(
                typeof(TEvent),
                (e, ctx, token) =>
                {
                    var result = callback((TEvent)e, ctx);
                    return Task.FromResult(result);
                });
        }
    }
}
