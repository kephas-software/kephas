// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEventPublisher.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEventPublisher interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Events
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Contract for the shared application service publishing events.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IEventPublisher
    {
        /// <summary>
        /// Asynchronously publishes the provided event.
        /// </summary>
        /// <param name="appEvent">The application event.</param>
        /// <param name="context">Optional. the context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        Task PublishAsync(IEvent appEvent, IContext context = null, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Extension methods for <see cref="IEventPublisher"/>.
    /// </summary>
    public static class AppEventEmitterExtensions
    {
        /// <summary>
        /// Publishes the provided event.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="eventPublisher">The event emitter to act on.</param>
        /// <param name="context">Optional. the context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static Task PublishAsync<TEvent>(
            this IEventPublisher eventPublisher,
            IContext context = null,
            CancellationToken cancellationToken = default)
            where TEvent : IEvent, new()
        {
            Requires.NotNull(eventPublisher, nameof(eventPublisher));

            return eventPublisher.PublishAsync(new TEvent(), context, cancellationToken);
        }


        /// <summary>
        /// Publishes the provided event.
        /// </summary>
        /// <param name="eventPublisher">The event emitter to act on.</param>
        /// <param name="appEvent">The application event.</param>
        /// <param name="context">Optional. the context.</param>
        public static void Publish(this IEventPublisher eventPublisher, IEvent appEvent, IContext context = null)
        {
            Requires.NotNull(eventPublisher, nameof(eventPublisher));

            if (eventPublisher is ISyncAppEventEmitter syncEventEmitter)
            {
                syncEventEmitter.Emit(appEvent, context);
            }
            else
            {
                eventPublisher.PublishAsync(appEvent, context).WaitNonLocking();
            }
        }

        /// <summary>
        /// Publishes the provided event.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="eventPublisher">The event emitter to act on.</param>
        /// <param name="context">Optional. the context.</param>
        public static void Publish<TEvent>(this IEventPublisher eventPublisher, IContext context = null)
            where TEvent : IEvent, new()
        {
            Requires.NotNull(eventPublisher, nameof(eventPublisher));

            var appEvent = new TEvent();
            if (eventPublisher is ISyncAppEventEmitter syncEventEmitter)
            {
                syncEventEmitter.Emit(appEvent, context);
            }
            else
            {
                eventPublisher.PublishAsync(appEvent, context).WaitNonLocking();
            }
        }

        /// <summary>
        /// Asynchronously publishes the event with the provided ID and arguments.
        /// </summary>
        /// <param name="eventPublisher">The event emitter to act on.</param>
        /// <param name="appEventId">Identifier for the application event.</param>
        /// <param name="appEventArgs">The application event arguments.</param>
        /// <param name="context">Optional. the context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static Task PublishAsync(
            this IEventPublisher eventPublisher,
            object appEventId,
            IExpando appEventArgs,
            IContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(eventPublisher, nameof(eventPublisher));

            var appEvent = new IdentifiableEvent { Id = appEventId, EventArgs = appEventArgs };
            return eventPublisher.PublishAsync(appEvent, context, cancellationToken);
        }

        /// <summary>
        /// Publishes the event with the provided ID and arguments.
        /// </summary>
        /// <param name="eventPublisher">The event emitter to act on.</param>
        /// <param name="appEventId">Identifier for the application event.</param>
        /// <param name="appEventArgs">The application event arguments.</param>
        /// <param name="context">Optional. the context.</param>
        public static void Publish(
            this IEventPublisher eventPublisher,
            object appEventId,
            IExpando appEventArgs,
            IContext context = null)
        {
            Requires.NotNull(eventPublisher, nameof(eventPublisher));

            var appEvent = new IdentifiableEvent { Id = appEventId, EventArgs = appEventArgs };
            eventPublisher.Publish(appEvent, context);
        }
    }
}