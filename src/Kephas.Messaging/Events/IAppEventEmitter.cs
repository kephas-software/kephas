// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppEventEmitter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppEventEmitter interface.
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
    /// Contract for the shared application service emitting events at application level.
    /// </summary>
    [SharedAppServiceContract]
    public interface IAppEventEmitter
    {
        /// <summary>
        /// Asynchronously emits the provided event.
        /// </summary>
        /// <param name="appEvent">The application event.</param>
        /// <param name="context">Optional. the context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        Task EmitAsync(IEvent appEvent, IContext context = null, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Extension methods for <see cref="IAppEventEmitter"/>.
    /// </summary>
    public static class AppEventEmitterExtensions
    {
        /// <summary>
        /// Emits the provided event.
        /// </summary>
        /// <param name="eventEmitter">The event emitter to act on.</param>
        /// <param name="appEvent">The application event.</param>
        /// <param name="context">Optional. the context.</param>
        public static void Emit(this IAppEventEmitter eventEmitter, IEvent appEvent, IContext context = null)
        {
            Requires.NotNull(eventEmitter, nameof(eventEmitter));

            if (eventEmitter is ISyncAppEventEmitter syncEventEmitter)
            {
                syncEventEmitter.Emit(appEvent, context);
            }
            else
            {
                eventEmitter.EmitAsync(appEvent, context).WaitNonLocking();
            }
        }

        /// <summary>
        /// Asynchronously emits the event with the provided ID and arguments.
        /// </summary>
        /// <param name="eventEmitter">The event emitter to act on.</param>
        /// <param name="appEventId">Identifier for the application event.</param>
        /// <param name="appEventArgs">The application event arguments.</param>
        /// <param name="context">Optional. the context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static Task EmitAsync(
            this IAppEventEmitter eventEmitter,
            object appEventId,
            IExpando appEventArgs,
            IContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(eventEmitter, nameof(eventEmitter));

            var appEvent = new IdentifiableEvent { Id = appEventId, EventArgs = appEventArgs };
            return eventEmitter.EmitAsync(appEvent, context, cancellationToken);
        }

        /// <summary>
        /// Emits the event with the provided ID and arguments.
        /// </summary>
        /// <param name="eventEmitter">The event emitter to act on.</param>
        /// <param name="appEventId">Identifier for the application event.</param>
        /// <param name="appEventArgs">The application event arguments.</param>
        /// <param name="context">Optional. the context.</param>
        public static void Emit(
            this IAppEventEmitter eventEmitter,
            object appEventId,
            IExpando appEventArgs,
            IContext context = null)
        {
            Requires.NotNull(eventEmitter, nameof(eventEmitter));

            var appEvent = new IdentifiableEvent { Id = appEventId, EventArgs = appEventArgs };
            eventEmitter.Emit(appEvent, context);
        }
    }
}