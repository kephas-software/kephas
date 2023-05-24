// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisAppMessageRouter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the redis message router class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Redis.Routing
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.Connectivity;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Distributed.Queues;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Redis.Configuration;
    using Kephas.Model.AttributedModel;
    using Kephas.Operations;
    using Kephas.Redis.Connectivity;
    using Kephas.Redis.Interaction;
    using Kephas.Serialization;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using StackExchange.Redis;

    /// <summary>
    /// The Redis message router.
    /// </summary>
    [Override]
    [ProcessingPriority(Priority.Low)]
    [MessageRouter(ReceiverMatch = ChannelType + ":.*", IsFallback = true)]
    public class RedisAppMessageRouter : DefaultAppMessageRouter
    {
        private readonly IConnectionProvider connectionProvider;
        private readonly ISerializationService serializationService;
        private readonly IConfiguration<RedisRoutingSettings> redisConfiguration;
        private readonly IEventHub eventHub;

        private readonly ConcurrentQueue<(TaskCompletionSource<(RoutingInstruction action, object? reply)> taskSource,
            Func<Task<(RoutingInstruction action, object? reply)>> asyncRouteAction)> preInitQueue = new();

        private ISubscriber? publisher;
        private IRedisConnection? subConnection;
        private ISubscriber? subscriber;
        private string? redisRootChannelName;
        private IRedisConnection? pubConnection;
        private OperationState redisChannelInitializationState = OperationState.NotStarted;
        private IEventSubscription? redisClientStartedSubscription;
        private IEventSubscription? redisClientStoppingSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisAppMessageRouter"/> class.
        /// </summary>
        /// <param name="injectableFactory">The injectable factory.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="queueStore">The message queue store.</param>
        /// <param name="connectionProvider">The connection provider.</param>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="redisConfiguration">The redis configuration.</param>
        /// <param name="eventHub">The event hub.</param>
        public RedisAppMessageRouter(
            IInjectableFactory injectableFactory,
            IAppRuntime appRuntime,
            IMessageProcessor messageProcessor,
            IMessageQueueStore queueStore,
            IConnectionProvider connectionProvider,
            ISerializationService serializationService,
            IConfiguration<RedisRoutingSettings> redisConfiguration,
            IEventHub eventHub)
            : base(injectableFactory, appRuntime, messageProcessor, queueStore)
        {
            this.connectionProvider = connectionProvider;
            this.serializationService = serializationService;
            this.redisConfiguration = redisConfiguration;
            this.eventHub = eventHub;
        }

        /// <summary>
        /// Initializes the router.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected override async Task InitializeCoreAsync(IContext? context, CancellationToken cancellationToken)
        {
            await base.InitializeCoreAsync(context, cancellationToken).PreserveThreadContext();

            await this.InitializeRedisChannelAsync(cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Initializes the Redis channel asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        protected virtual async Task InitializeRedisChannelAsync(CancellationToken cancellationToken)
        {
            this.redisClientStartedSubscription?.Dispose();
            this.redisClientStartedSubscription = null;

            this.redisClientStoppingSubscription = this.eventHub.Subscribe<ConnectionManagerStoppingSignal>((e, ctx) => this.DisposeRedisChannel(e));

            try
            {
                this.Logger.Info($"Redis initialized, starting initialization of the Redis channel...");

                var redisSettings = this.redisConfiguration.GetSettings(this.AppContext);
                var redisNamespace = redisSettings.Namespace;
                this.redisRootChannelName = string.IsNullOrEmpty(redisNamespace) ? ChannelType : $"{redisNamespace}:{ChannelType}";

                var connectionUri = redisSettings.ConnectionUri;
                if (string.IsNullOrEmpty(connectionUri))
                {
                    throw new RoutingException($"The connection URI is not set in '{typeof(RedisRoutingSettings)}'.");
                }

                var connection = this.connectionProvider.CreateConnection(connectionUri, options: ctx => ctx.Impersonate(this.AppContext));
                this.pubConnection = connection as IRedisConnection;
                if (this.pubConnection is null)
                {
                    throw new RoutingException($"Expected a {typeof(IRedisConnection)}, but received a {connection?.GetType()}.");
                }
                this.publisher = this.pubConnection.Of.GetSubscriber();

                connection = this.connectionProvider.CreateConnection(connectionUri, options: ctx => ctx.Impersonate(this.AppContext));
                this.subConnection = connection as IRedisConnection;
                if (this.subConnection is null)
                {
                    throw new RoutingException($"Expected a {typeof(IRedisConnection)}, but received a {connection?.GetType()}.");
                }
                this.subscriber = this.subConnection.Of.GetSubscriber();

                await this.subscriber.SubscribeAsync(this.redisRootChannelName, this.HandleOnMessage).PreserveThreadContext();
                await this.subscriber.SubscribeAsync($"{this.redisRootChannelName}:{this.AppRuntime.GetAppId()}", this.HandleOnMessage).PreserveThreadContext();
                await this.subscriber.SubscribeAsync($"{this.redisRootChannelName}:{this.AppRuntime.GetAppInstanceId()}", this.HandleOnMessage).PreserveThreadContext();

                this.Logger.Info($"Completed initialization of the Redis channel.");

                this.redisChannelInitializationState = OperationState.Completed;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Redis connection could not be created.");
                this.redisChannelInitializationState = OperationState.Failed;
            }

            // all the entries in the pre init queue should be processed now.
            await this.PostInitializeRedisChannelAsync().PreserveThreadContext();
        }

        /// <summary>
        /// Calculates the root channel name.
        /// </summary>
        /// <returns>
        /// The calculated root channel name.
        /// </returns>
        protected override string ComputeRootChannelName() => $"{this.GetType().Name}-{base.ComputeRootChannelName()}";

        /// <summary>
        /// Receives a message asynchronously.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="value">The value.</param>
        protected virtual void HandleOnMessage(RedisChannel channel, RedisValue value)
        {
            var serializedMessage = (string)value;
            try
            {
                var message = this.serializationService.JsonDeserialize(serializedMessage);
                if (message is IBrokeredMessage brokeredMessage)
                {
                    this.RouteInputAsync(brokeredMessage, this.AppContext, default)
                        .ContinueWith(
                            t => this.Logger.Error(t.Exception, "Error while routing from input '{message}'.", brokeredMessage),
                            TaskContinuationOptions.OnlyOnFaulted);
                }
                else
                {
                    this.Logger.Warn("Unsupported message '{message}'.", serializedMessage);
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while handling message '{message}'.", serializedMessage);
            }
        }

        /// <summary>
        /// Routes the brokered message asynchronously, typically over the physical medium.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The dispatching context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding an action to take further and an optional reply.
        /// </returns>
        protected override Task<(RoutingInstruction action, object? reply)> RouteOutputAsync(
            IBrokeredMessage brokeredMessage, IDispatchingContext context, CancellationToken cancellationToken)
        {
            this.InitializationMonitor.AssertIsCompletedSuccessfully();

            async Task<(RoutingInstruction action, object? reply)> RouteOutputCoreAsync()
            {
                if (brokeredMessage.Recipients?.Any() ?? false)
                {
                    var groups = brokeredMessage.Recipients
                        .GroupBy(r => this.GetChannelName(r))
                        .Select(g => (channelName: g.Key, recipients: g))
                        .ToList();

                    if (groups.Count == 1)
                    {
                        var serializedMessage = await this.serializationService
                            .SerializeAsync(brokeredMessage, ctx => ctx.IncludeTypeInfo(true), cancellationToken)
                            .PreserveThreadContext();
                        await this.PublishAsync(serializedMessage, groups[0].channelName, brokeredMessage.IsOneWay)
                            .PreserveThreadContext();
                    }
                    else
                    {
                        foreach (var (channelName, recipients) in groups)
                        {
                            var serializedMessage = await this.serializationService
                                .SerializeAsync(brokeredMessage.Clone(recipients), ctx => ctx.IncludeTypeInfo(true), cancellationToken)
                                .PreserveThreadContext();
                            await this.PublishAsync(serializedMessage, channelName, brokeredMessage.IsOneWay)
                                .PreserveThreadContext();
                        }
                    }
                }
                else
                {
                    var serializedMessage = await this.serializationService
                        .SerializeAsync(brokeredMessage, ctx => ctx.IncludeTypeInfo(true), cancellationToken)
                        .PreserveThreadContext();
                    await this.PublishAsync(serializedMessage, this.redisRootChannelName!, brokeredMessage.IsOneWay)
                        .PreserveThreadContext();
                }

                return (RoutingInstruction.None, null);
            }

            switch (this.redisChannelInitializationState)
            {
                case OperationState.Failed:
                    // if the Redis channel failed to initialize, leave the base route the message.
                    return base.RouteOutputAsync(brokeredMessage, context, cancellationToken);
                case OperationState.Completed:
                    // if the Redis channel is initialized, use it.
                    return RouteOutputCoreAsync();
                default:
                    // otherwise postpone the execution until the channel gets initialized.
                    var taskCompletionSource = new TaskCompletionSource<(RoutingInstruction action, object? reply)>();
                    this.preInitQueue.Enqueue((taskCompletionSource, () => RouteOutputCoreAsync()));
                    return brokeredMessage.IsOneWay
                        ? Task.FromResult<(RoutingInstruction action, object? reply)>((RoutingInstruction.None, null))
                        : taskCompletionSource.Task;
            }
        }

        /// <summary>
        /// Gets the channel name for the provided recipient.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <returns>
        /// The channel name.
        /// </returns>
        protected override string GetChannelName(IEndpoint recipient)
        {
            if (this.redisChannelInitializationState != OperationState.Completed)
            {
                return base.GetChannelName(recipient);
            }

            return string.IsNullOrEmpty(recipient.AppInstanceId)
                        ? string.IsNullOrEmpty(recipient.AppId)
                            ? this.redisRootChannelName!
                            : $"{this.redisRootChannelName}:{recipient.AppId}"
                        : $"{this.redisRootChannelName}:{recipient.AppInstanceId}";
        }

        /// <summary>
        /// Releases the unmanaged resources used by the MessageRouterBase and optionally releases the
        /// managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                this.DisposeRedisChannel(new ConnectionManagerStoppingSignal());
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Disposes the Redis channel.
        /// </summary>
        /// <param name="signal">The Redis client stopping signal.</param>
        protected virtual void DisposeRedisChannel(ConnectionManagerStoppingSignal signal)
        {
            this.redisClientStartedSubscription?.Dispose();
            this.redisClientStartedSubscription = null;

            this.redisClientStoppingSubscription?.Dispose();
            this.redisClientStoppingSubscription = null;

            if (this.redisChannelInitializationState == OperationState.Completed)
            {
                this.pubConnection?.Dispose();
                this.pubConnection = null;

                try
                {
                    this.subscriber?.UnsubscribeAll();
                }
                catch (OperationCanceledException ex)
                {
                    this.Logger.Warn(ex, $"Redis subscription cancellation was canceled.");
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, $"Errors occured during Redis subscription cancellation.");
                }

                this.subConnection?.Dispose();
                this.subConnection = null;

                this.redisChannelInitializationState = OperationState.NotStarted;
            }
        }

        private async Task PostInitializeRedisChannelAsync()
        {
            while (this.preInitQueue.TryDequeue(out var queueEntry))
            {
                try
                {
                    var result = await queueEntry.asyncRouteAction().PreserveThreadContext();
                    queueEntry.taskSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    queueEntry.taskSource.SetException(ex);
                }
            }
        }

        private async Task PublishAsync(string serializedMessage, string channelName, bool oneWay)
        {
            if (this.publisher == null)
            {
                throw new InvalidOperationException("The publisher is not set.");
            }

            if (oneWay)
            {
                await this.publisher!.PublishAsync(channelName, serializedMessage, CommandFlags.FireAndForget).PreserveThreadContext();
            }
            else
            {
                await this.publisher!.PublishAsync(channelName, serializedMessage).PreserveThreadContext();
            }
        }
    }
}