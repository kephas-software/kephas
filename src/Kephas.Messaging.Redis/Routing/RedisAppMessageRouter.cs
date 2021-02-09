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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.ExceptionHandling;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Model.AttributedModel;
    using Kephas.Redis;
    using Kephas.Redis.Configuration;
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
    public class RedisAppMessageRouter : InProcessAppMessageRouter
    {
        private readonly IRedisConnectionManager redisConnectionManager;
        private readonly ISerializationService serializationService;
        private readonly IConfiguration<RedisClientSettings> redisConfiguration;
        private readonly IEventHub eventHub;
        private ISubscriber publisher;
        private IConnectionMultiplexer? subConnection;
        private ISubscriber subscriber;
        private string redisRootChannelName;
        private IConnectionMultiplexer? pubConnection;
        private bool isRedisChannelInitialized;
        private IEventSubscription? redisClientStartedSubscription;
        private IEventSubscription? redisClientStoppingSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisAppMessageRouter"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="redisConnectionManager">The Redis connection manager.</param>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="redisConfiguration">The redis configuration.</param>
        /// <param name="eventHub">The event hub.</param>
        public RedisAppMessageRouter(
            IContextFactory contextFactory,
            IAppRuntime appRuntime,
            IMessageProcessor messageProcessor,
            IRedisConnectionManager redisConnectionManager,
            ISerializationService serializationService,
            IConfiguration<RedisClientSettings> redisConfiguration,
            IEventHub eventHub)
            : base(contextFactory, appRuntime, messageProcessor)
        {
            this.redisConnectionManager = redisConnectionManager;
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

            if (!this.redisConnectionManager.IsInitialized)
            {
                this.Logger.Info($"Redis client not initialized, postponing initialization of the Redis channel.");

                this.redisClientStartedSubscription = this.eventHub.Subscribe<ConnectionManagerStartedSignal>((e, ctx, ct) => this.InitializeRedisChannelAsync(e, ct));

                return;
            }

            await this.InitializeRedisChannelAsync(new ConnectionManagerStartedSignal(), cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Initializes the Redis channel asynchronously.
        /// </summary>
        /// <param name="signal">The Redis client started signal.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        protected virtual async Task InitializeRedisChannelAsync(ConnectionManagerStartedSignal signal, CancellationToken cancellationToken)
        {
            this.redisClientStartedSubscription?.Dispose();
            this.redisClientStartedSubscription = null;

            this.redisClientStoppingSubscription = this.eventHub.Subscribe<ConnectionManagerStoppingSignal>((e, ctx) => this.DisposeRedisChannel(e));

            if (signal.Severity.IsError())
            {
                this.Logger.Info($"Redis client initialization failed, cancelling initialization of the Redis channel.");
                return;
            }

            this.Logger.Info($"Redis initialized, starting initialization of the Redis channel...");

            var redisNS = this.redisConfiguration.GetSettings(this.AppContext).Namespace;
            this.redisRootChannelName = string.IsNullOrEmpty(redisNS) ? ChannelType : $"{redisNS}:{ChannelType}";

            this.pubConnection = this.redisConnectionManager.CreateConnection();
            this.publisher = this.pubConnection.GetSubscriber();

            this.subConnection = this.redisConnectionManager.CreateConnection();
            this.subscriber = this.subConnection.GetSubscriber();

            await this.subscriber.SubscribeAsync(this.redisRootChannelName, this.HandleOnMessage).PreserveThreadContext();
            await this.subscriber.SubscribeAsync($"{this.redisRootChannelName}:{this.AppRuntime.GetAppId()}", this.HandleOnMessage).PreserveThreadContext();
            await this.subscriber.SubscribeAsync($"{this.redisRootChannelName}:{this.AppRuntime.GetAppInstanceId()}", this.HandleOnMessage).PreserveThreadContext();

            this.Logger.Info($"Completed initialization of the Redis channel.");

            this.isRedisChannelInitialized = true;
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
        protected override async Task<(RoutingInstruction action, IMessage? reply)> RouteOutputAsync(IBrokeredMessage brokeredMessage, IDispatchingContext context, CancellationToken cancellationToken)
        {
            this.InitializationMonitor.AssertIsCompletedSuccessfully();

            if (!this.isRedisChannelInitialized)
            {
                return await base.RouteOutputAsync(brokeredMessage, context, cancellationToken).PreserveThreadContext();
            }

            if (brokeredMessage.Recipients?.Any() ?? false)
            {
                var groups = brokeredMessage.Recipients
                    .GroupBy(r => this.GetChannelName(r))
                    .Select(g => (channelName: g.Key, recipients: g))
                    .ToList();

                if (groups.Count == 1)
                {
                    var serializedMessage = await this.serializationService.SerializeAsync(brokeredMessage, ctx => ctx.IncludeTypeInfo(true), cancellationToken).PreserveThreadContext();
                    await this.PublishAsync(serializedMessage, groups[0].channelName, brokeredMessage.IsOneWay).PreserveThreadContext();
                }
                else
                {
                    foreach (var (channelName, recipients) in groups)
                    {
                        var serializedMessage = await this.serializationService.SerializeAsync(brokeredMessage.Clone(recipients), ctx => ctx.IncludeTypeInfo(true), cancellationToken).PreserveThreadContext();
                        await this.PublishAsync(serializedMessage, channelName, brokeredMessage.IsOneWay).PreserveThreadContext();
                    }
                }
            }
            else
            {
                var serializedMessage = await this.serializationService.SerializeAsync(brokeredMessage, ctx => ctx.IncludeTypeInfo(true), cancellationToken).PreserveThreadContext();
                await this.PublishAsync(serializedMessage, this.redisRootChannelName, brokeredMessage.IsOneWay).PreserveThreadContext();
            }

            return (RoutingInstruction.None, null);
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
            if (!this.isRedisChannelInitialized)
            {
                return base.GetChannelName(recipient);
            }

            return string.IsNullOrEmpty(recipient.AppInstanceId)
                        ? string.IsNullOrEmpty(recipient.AppId)
                            ? this.redisRootChannelName
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

            if (this.isRedisChannelInitialized)
            {
                this.redisConnectionManager.DisposeConnection(this.pubConnection!);
                this.pubConnection = null;

                try
                {
                    this.subscriber.UnsubscribeAll();
                }
                catch (OperationCanceledException ex)
                {
                    this.Logger.Warn(ex, $"Redis subscription cancellation was canceled.");
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, $"Errors occured during Redis subscription cancellation.");
                }

                this.redisConnectionManager.DisposeConnection(this.subConnection!);
                this.subConnection = null;

                this.isRedisChannelInitialized = false;
            }
        }

        private async Task PublishAsync(string serializedMessage, string channelName, bool oneWay)
        {
            if (oneWay)
            {
                await this.publisher.PublishAsync(channelName, serializedMessage, CommandFlags.FireAndForget).PreserveThreadContext();
            }
            else
            {
                await this.publisher.PublishAsync(channelName, serializedMessage).PreserveThreadContext();
            }
        }
    }
}