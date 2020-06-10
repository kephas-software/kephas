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
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
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
        /// An asynchronous result.
        /// </returns>
        Task PublishAsync(object @event, IContext context, CancellationToken cancellationToken = default);

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
        /// Subscribes to the event(s) matching the provided type.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="eventHub">The event hub to act on.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        public static IEventSubscription Subscribe<TEvent>(this IEventHub eventHub, Func<TEvent, IContext, CancellationToken, Task> callback)
            where TEvent : class
        {
            Requires.NotNull(eventHub, nameof(eventHub));
            Requires.NotNull(callback, nameof(callback));

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
        public static IEventSubscription Subscribe<TEvent>(this IEventHub eventHub, Action<TEvent, IContext> callback)
            where TEvent : class
        {
            Requires.NotNull(eventHub, nameof(eventHub));
            Requires.NotNull(callback, nameof(callback));

            return eventHub.Subscribe(
                typeof(TEvent),
                (e, ctx, token) =>
                    {
                        callback((TEvent)e, ctx);
                        return Task.CompletedTask;
                    });
        }
    }
}
