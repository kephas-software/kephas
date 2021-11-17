// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultEventHub.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default event hub class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Interaction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Dynamic;
    using Kephas.ExceptionHandling;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Resources;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The default implementation of the <see cref="IEventHub"/> service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultEventHub : Loggable, IEventHub, IDisposable
    {
        private readonly IContextFactory contextFactory;
        private readonly IList<EventSubscription> subscriptions = new List<EventSubscription>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEventHub"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        public DefaultEventHub(IContextFactory contextFactory, ILogManager? logManager = null)
            : base(logManager)
        {
            this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

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
        public virtual async Task<IOperationResult<IEnumerable<object?>>> PublishAsync(object @event, IContext? context = null, CancellationToken cancellationToken = default)
        {
            @event = @event ?? throw new ArgumentNullException(nameof(@event));

            IList<EventSubscription> subscriptionsCopy;
            lock (this.subscriptions)
            {
                subscriptionsCopy = this.subscriptions.Where(s => s.Match(@event)).ToList();
            }

            var resultList = new List<object?>();
            var result = new OperationResult<IEnumerable<object?>>
            {
                OperationState = OperationState.Completed,
                Value = resultList,
            };
            foreach (var subscription in subscriptionsCopy)
            {
                try
                {
                    var task = subscription.Callback?.Invoke(this.GetEventContent(@event), context, cancellationToken);
                    if (task != null)
                    {
                        await task.PreserveThreadContext();
                        resultList.Add(task.GetResult());
                    }
                }
                catch (InterruptSignal interruption)
                {
                    switch (interruption.Severity)
                    {
                        case SeverityLevel.Info:
                            resultList.Add(interruption.Result);
                            result.Complete();
                            break;
                        case SeverityLevel.Warning:
                            resultList.Add(interruption.Result);
                            result.Complete(operationState: OperationState.Warning);
                            this.Logger.Warn(interruption.InnerException ?? interruption, "A warning interruption occurred when invoking subscription callback for '{event}'.", @event);
                            break;
                        default:
                            result.Fail(interruption.InnerException ?? interruption, operationState: OperationState.Aborted);
                            this.Logger.Log((LogLevel)interruption.Severity, interruption.InnerException ?? interruption, AbstractionStrings.DefaultEventHub_ErrorWhenInvokingSubscriptionCallback, @event);
                            break;
                    }

                    break;
                }
                catch (Exception ex)
                {
                    result.Fail(ex);
                    this.Logger.Error(ex, AbstractionStrings.DefaultEventHub_ErrorWhenInvokingSubscriptionCallback, @event);
                }
            }

            return result.Complete(operationState: result.HasErrors() ? OperationState.Failed : OperationState.Completed);
        }

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
        public Task<IOperationResult<IEnumerable<object?>>> PublishAsync<TContext>(object @event, Action<TContext> options, CancellationToken cancellationToken = default)
            where TContext : class, IContext
        {
            var context = this.contextFactory.CreateContext<TContext>().Merge(options);
            return this.PublishAsync(@event, context, cancellationToken);
        }

        /// <summary>
        /// Subscribes to the event(s) matching the criteria.
        /// </summary>
        /// <param name="match">Specifies the event match criteria.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        public virtual IEventSubscription Subscribe(Func<object, bool> match, Func<object, IContext?, CancellationToken, Task> callback)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            callback = callback ?? throw new ArgumentNullException(nameof(callback));

            var subscription = new EventSubscription(
                match,
                callback,
                s =>
                {
                    lock (this.subscriptions)
                    {
                        this.subscriptions.Remove(s);
                    }
                });
            lock (this.subscriptions)
            {
                this.subscriptions.Add(subscription);
            }

            return subscription;
        }

        /// <summary>
        /// Subscribes to the event(s) matching the provided type.
        /// </summary>
        /// <param name="typeMatch">Specifies the type match criteria.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        public virtual IEventSubscription Subscribe(Type typeMatch, Func<object, IContext?, CancellationToken, Task> callback)
        {
            typeMatch = typeMatch ?? throw new ArgumentNullException(nameof(typeMatch));

            return this.Subscribe(this.GetTypeMatch(typeMatch), callback);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the Kephas.Interaction.DefaultEventHub and
        /// optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            lock (this.subscriptions)
            {
                this.subscriptions.Clear();
            }
        }

        /// <summary>
        /// Gets the event content.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <returns>
        /// The event content.
        /// </returns>
        protected virtual object GetEventContent(object @event)
        {
            return @event;
        }

        /// <summary>
        /// Gets the match for the provided event type.
        /// </summary>
        /// <param name="typeMatch">Specifies the type match criteria.</param>
        /// <returns>
        /// A function delegate that yields a bool.
        /// </returns>
        protected virtual Func<object, bool> GetTypeMatch(Type typeMatch)
        {
            return e => typeMatch == e.GetType();
        }

        /// <summary>
        /// An event subscription.
        /// </summary>
        private class EventSubscription : IEventSubscription
        {
            private readonly Action<EventSubscription> onDispose;
            private bool disposed;

            /// <summary>
            /// Initializes a new instance of the <see cref="EventSubscription"/> class.
            /// </summary>
            /// <param name="match">Specifies the event match.</param>
            /// <param name="callback">The callback.</param>
            /// <param name="onDispose">The on dispose.</param>
            public EventSubscription(Func<object, bool> match, Func<object, IContext?, CancellationToken, Task> callback, Action<EventSubscription> onDispose)
            {
                this.Match = match;
                this.Callback = callback;
                this.onDispose = onDispose;
            }

            /// <summary>
            /// Gets the event match.
            /// </summary>
            /// <value>
            /// The match.
            /// </value>
            public Func<object, bool> Match { get; }

            /// <summary>
            /// Gets the callback.
            /// </summary>
            /// <value>
            /// The callback.
            /// </value>
            public Func<object, IContext?, CancellationToken, Task> Callback { get; private set; }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
            /// resources.
            /// </summary>
            public void Dispose()
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposed = true;
                this.onDispose(this);
            }

            /// <summary>Returns a string that represents the current object.</summary>
            /// <returns>A string that represents the current object.</returns>
            public override string ToString()
            {
                return $"{this.GetType().Name}:{this.Match}";
            }
        }
    }
}
