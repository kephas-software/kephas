// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisMessageRouter.cs" company="Kephas Software SRL">
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
    [ProcessingPriority(Priority.Low)]
    [MessageRouter(ReceiverMatch = ChannelType + ":.*", IsFallback = true)]
    public class RedisAppMessageRouter : InProcessAppMessageRouter
    {
        private readonly IRedisClient redisClient;
        private readonly ISerializationService serializationService;
        private readonly IConfiguration<RedisClientSettings> redisConfiguration;
        private readonly IEventHub eventHub;
        private ISubscriber subscriber;
        private ChannelMessageQueue messageQueue;
        private ChannelMessageQueue appMessageQueue;
        private ChannelMessageQueue appInstanceMessageQueue;
        private string redisRootChannelName;
        private bool isRedisChannelInitialized;
        private IEventSubscription redisInitializedSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisAppMessageRouter"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="redisClient">The redis client.</param>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="redisConfiguration">The redis configuration.</param>
        /// <param name="eventHub">The event hub.</param>
        public RedisAppMessageRouter(
            IContextFactory contextFactory,
            IAppRuntime appRuntime,
            IMessageProcessor messageProcessor,
            IRedisClient redisClient,
            ISerializationService serializationService,
            IConfiguration<RedisClientSettings> redisConfiguration,
            IEventHub eventHub)
            : base(contextFactory, appRuntime, messageProcessor)
        {
            this.redisClient = redisClient;
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
        protected override async Task InitializeCoreAsync(IContext context, CancellationToken cancellationToken = default)
        {
            await base.InitializeCoreAsync(context, cancellationToken).PreserveThreadContext();

            await this.InitializeRedisChannelAsync(context, cancellationToken).PreserveThreadContext();
        }

        /// <summary>Initializes the Redis channel asynchronously.</summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task.</returns>
        protected virtual async Task InitializeRedisChannelAsync(IContext context, CancellationToken cancellationToken)
        {
            async Task InitializeAsync(RedisClientInitializedSignal initSignal, CancellationToken token)
            {
                this.redisInitializedSubscription?.Dispose();
                this.redisInitializedSubscription = null;

                if (initSignal.Severity.IsError())
                {
                    this.Logger.Info($"Redis client initialization failed, cancelling initialization of the Redis channel.");
                    return;
                }

                this.Logger.Info($"Redis initialized, starting initialization of the Redis channel...");

                var redisNS = this.redisConfiguration.Settings.Namespace;
                this.redisRootChannelName = string.IsNullOrEmpty(redisNS) ? ChannelType : $"{redisNS}:{ChannelType}";

                var connection = this.redisClient.GetConnection();

                this.subscriber = connection.GetSubscriber();
                this.messageQueue = await this.subscriber.SubscribeAsync(this.redisRootChannelName).PreserveThreadContext();
                this.messageQueue.OnMessage(this.ReceiveMessageAsync);

                this.appMessageQueue = await this.subscriber.SubscribeAsync($"{this.redisRootChannelName}:{this.AppRuntime.GetAppId()}").PreserveThreadContext();
                this.appMessageQueue.OnMessage(this.ReceiveMessageAsync);

                this.appInstanceMessageQueue = await this.subscriber.SubscribeAsync($"{this.redisRootChannelName}:{this.AppRuntime.GetAppInstanceId()}").PreserveThreadContext();
                this.appInstanceMessageQueue.OnMessage(this.ReceiveMessageAsync);

                this.Logger.Info($"Completed initialization of the Redis channel.");

                this.isRedisChannelInitialized = true;
            }

            if (!this.redisClient.IsInitialized)
            {
                this.Logger.Info($"Redis client not initialized, postponing initialization of the Redis channel.");

                this.redisInitializedSubscription = this.eventHub.Subscribe<RedisClientInitializedSignal>((s, ctx, ct) => InitializeAsync(s, ct));

                return;
            }

            await InitializeAsync(new RedisClientInitializedSignal(), cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Calculates the root channel name.
        /// </summary>
        /// <returns>
        /// The calculated root channel name.
        /// </returns>
        protected override string ComputeRootChannelName() => $"Redis-{base.ComputeRootChannelName()}";

        /// <summary>
        /// Receives a message asynchronously.
        /// </summary>
        /// <param name="channelMessage">The channel message.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task ReceiveMessageAsync(ChannelMessage channelMessage)
        {
            var serializedMessage = (string)channelMessage.Message;
            try
            {
                var message = await this.serializationService.JsonDeserializeAsync(serializedMessage).PreserveThreadContext();
                if (message is IBrokeredMessage brokeredMessage)
                {
                    await this.RouteInputAsync(brokeredMessage, this.AppContext, default).PreserveThreadContext();
                }
                else
                {
                    this.Logger.Warn($"Unsupported message '{serializedMessage}'.");
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, $"Error while handling message '{serializedMessage}'.");
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
        protected override async Task<(RoutingInstruction action, IMessage reply)> RouteOutputAsync(IBrokeredMessage brokeredMessage, IDispatchingContext context, CancellationToken cancellationToken)
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
                    var serializedMessage = await this.serializationService.SerializeAsync(brokeredMessage, ctx => ctx.IncludeTypeInfo(true)).PreserveThreadContext();
                    await this.PublishAsync(serializedMessage, groups[0].channelName, brokeredMessage.IsOneWay).PreserveThreadContext();
                }
                else
                {
                    foreach (var group in groups)
                    {
                        var serializedMessage = await this.serializationService.SerializeAsync(brokeredMessage.Clone(group.recipients), ctx => ctx.IncludeTypeInfo(true)).PreserveThreadContext();
                        await this.PublishAsync(serializedMessage, group.channelName, brokeredMessage.IsOneWay).PreserveThreadContext();
                    }
                }
            }
            else
            {
                var serializedMessage = await this.serializationService.SerializeAsync(brokeredMessage, ctx => ctx.IncludeTypeInfo(true)).PreserveThreadContext();
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
        /// Actual implementation of the router disposal.
        /// </summary>
        protected override void DisposeCore()
        {
            try
            {
                this.DisposeRedisCore();
            }
            finally
            {
                base.DisposeCore();
            }
        }

        /// <summary>
        /// Actual implementation of the Redis channel disposal.
        /// </summary>
        protected virtual void DisposeRedisCore()
        {
            this.redisInitializedSubscription?.Dispose();
            this.redisInitializedSubscription = null;

            if (this.isRedisChannelInitialized)
            {
                this.messageQueue.Unsubscribe();
                this.appMessageQueue.Unsubscribe();
                this.appInstanceMessageQueue.Unsubscribe();
            }
        }

        private async Task PublishAsync(string serializedMessage, string channelName, bool oneWay)
        {
            if (oneWay)
            {
                await this.subscriber.PublishAsync(channelName, serializedMessage, CommandFlags.FireAndForget).PreserveThreadContext();
            }
            else
            {
                await this.subscriber.PublishAsync(channelName, serializedMessage).PreserveThreadContext();
            }
        }
    }
}