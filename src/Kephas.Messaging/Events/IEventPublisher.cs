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
        /// <param name="event">The event to be published.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        Task PublishAsync(object @event, IContext context, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Extension methods for <see cref="IEventPublisher"/>.
    /// </summary>
    public static class EventPublisherExtensions
    {
        /// <summary>
        /// Publishes the provided event.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="eventPublisher">The event emitter to act on.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static Task PublishAsync<TEvent>(
            this IEventPublisher eventPublisher,
            IContext context,
            CancellationToken cancellationToken = default)
            where TEvent : class, new()
        {
            Requires.NotNull(eventPublisher, nameof(eventPublisher));
            Requires.NotNull(context, nameof(context));

            return eventPublisher.PublishAsync(new TEvent(), context, cancellationToken);
        }


        /// <summary>
        /// Publishes the provided event.
        /// </summary>
        /// <param name="eventPublisher">The event emitter to act on.</param>
        /// <param name="event">The event.</param>
        /// <param name="context">The context.</param>
        public static void Publish(this IEventPublisher eventPublisher, object @event, IContext context)
        {
            Requires.NotNull(eventPublisher, nameof(eventPublisher));
            Requires.NotNull(context, nameof(context));

            if (eventPublisher is ISyncEventPublisher syncEventEmitter)
            {
                syncEventEmitter.Publish(@event, context);
            }
            else
            {
                eventPublisher.PublishAsync(@event, context).WaitNonLocking();
            }
        }

        /// <summary>
        /// Publishes the provided event.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="eventPublisher">The event emitter to act on.</param>
        /// <param name="context">The context.</param>
        public static void Publish<TEvent>(this IEventPublisher eventPublisher, IContext context)
            where TEvent : class, new()
        {
            Requires.NotNull(eventPublisher, nameof(eventPublisher));
            Requires.NotNull(context, nameof(context));

            var @event = new TEvent();
            if (eventPublisher is ISyncEventPublisher syncEventEmitter)
            {
                syncEventEmitter.Publish(@event, context);
            }
            else
            {
                eventPublisher.PublishAsync(@event, context).WaitNonLocking();
            }
        }

        /// <summary>
        /// Asynchronously publishes the event with the provided ID and arguments.
        /// </summary>
        /// <param name="eventPublisher">The event emitter to act on.</param>
        /// <param name="eventId">Identifier for the event.</param>
        /// <param name="eventArgs">The application event arguments.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static Task PublishAsync(
            this IEventPublisher eventPublisher,
            object eventId,
            IExpando eventArgs,
            IContext context,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(eventPublisher, nameof(eventPublisher));
            Requires.NotNull(context, nameof(context));

            var appEvent = new IdentifiableEvent { Id = eventId, EventArgs = eventArgs };
            return eventPublisher.PublishAsync(appEvent, context, cancellationToken);
        }

        /// <summary>
        /// Publishes the event with the provided ID and arguments.
        /// </summary>
        /// <param name="eventPublisher">The event emitter to act on.</param>
        /// <param name="eventId">Identifier for the application event.</param>
        /// <param name="eventArgs">The application event arguments.</param>
        /// <param name="context">Optional. the context.</param>
        public static void Publish(
            this IEventPublisher eventPublisher,
            object eventId,
            IExpando eventArgs,
            IContext context)
        {
            Requires.NotNull(eventPublisher, nameof(eventPublisher));
            Requires.NotNull(context, nameof(context));

            var @event = new IdentifiableEvent { Id = eventId, EventArgs = eventArgs };
            if (eventPublisher is ISyncEventPublisher syncEventEmitter)
            {
                syncEventEmitter.Publish(@event, context);
            }
            else
            {
                eventPublisher.PublishAsync(@event, context).WaitNonLocking();
            }
        }
    }
}