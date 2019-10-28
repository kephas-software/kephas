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
    using Kephas.Composition;
    using Kephas.Configuration;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Redis;
    using Kephas.Redis.Configuration;
    using Kephas.Serialization;
    using Kephas.Services;
    using Kephas.Services.Transitioning;
    using Kephas.Threading.Tasks;
    using StackExchange.Redis;

    /// <summary>
    /// The Redis message router.
    /// </summary>
    [MessageRouter(ReceiverUrlRegex = ChannelType + ":.*", IsFallback = true)]
    public class RedisMessageRouter : MessageRouterBase, IAsyncInitializable
    {
        private const string ChannelType = "app";

        private readonly InitializationMonitor<RedisMessageRouter, RedisMessageRouter> initializationMonitor = new InitializationMonitor<RedisMessageRouter, RedisMessageRouter>();
        private readonly FinalizationMonitor<RedisMessageRouter, RedisMessageRouter> finalizationMonitor = new FinalizationMonitor<RedisMessageRouter, RedisMessageRouter>();
        private readonly IRedisClient redisClient;
        private readonly ISerializationService serializationService;
        private readonly IConfiguration<RedisClientSettings> redisConfiguration;
        private ISubscriber subscriber;
        private IContext appContext;
        private ChannelMessageQueue messageQueue;
        private ChannelMessageQueue appMessageQueue;
        private ChannelMessageQueue appInstanceMessageQueue;
        private string rootChannelName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisMessageRouter"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="redisClient">The redis client.</param>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="messageBuilderFactory">The message builder factory.</param>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="redisConfiguration">The redis configuration.</param>
        public RedisMessageRouter(
            IAppRuntime appRuntime,
            IRedisClient redisClient,
            IMessageProcessor messageProcessor,
            IExportFactory<IBrokeredMessageBuilder> messageBuilderFactory,
            ISerializationService serializationService,
            IConfiguration<RedisClientSettings> redisConfiguration)
            : base(messageProcessor, messageBuilderFactory)
        {
            this.redisClient = redisClient;
            this.serializationService = serializationService;
            this.redisConfiguration = redisConfiguration;
            this.AppRuntime = appRuntime;
        }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        public IAppRuntime AppRuntime { get; }

        /// <summary>Initializes the service asynchronously.</summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task.</returns>
        public async Task InitializeAsync(IContext context = null, CancellationToken cancellationToken = default)
        {
            if (!this.redisClient.IsInitialized)
            {
                return;
            }

            this.initializationMonitor.AssertIsNotStarted();

            this.Logger.Info("Redis initialized, starting the Redis message router...");

            var messageRouterName = this.GetType().Name;

            try
            {
                this.initializationMonitor.Start();

                this.Logger.Info($"Initializing the {messageRouterName}...");

                var redisNS = this.redisConfiguration.Settings.Namespace;
                this.rootChannelName = string.IsNullOrEmpty(redisNS) ? ChannelType : $"{redisNS}:{ChannelType}";

                this.appContext = context;
                var connection = this.redisClient.GetConnection();

                this.Logger.Info($"Starting the {messageRouterName}...");

                this.subscriber = connection.GetSubscriber();
                this.messageQueue = await this.subscriber.SubscribeAsync(this.rootChannelName).PreserveThreadContext();
                this.messageQueue.OnMessage(this.ReceiveMessageAsync);

                this.appMessageQueue = await this.subscriber.SubscribeAsync($"{this.rootChannelName}:{this.AppRuntime.GetAppId()}").PreserveThreadContext();
                this.appMessageQueue.OnMessage(this.ReceiveMessageAsync);

                this.appInstanceMessageQueue = await this.subscriber.SubscribeAsync($"{this.rootChannelName}:{this.AppRuntime.GetAppInstanceId()}").PreserveThreadContext();
                this.appInstanceMessageQueue.OnMessage(this.ReceiveMessageAsync);

                this.Logger.Info($"{messageRouterName} started.");

                this.initializationMonitor.Complete();
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, $"{messageRouterName} failed to initialize.");
                this.initializationMonitor.Fault(ex);
                throw;
            }
        }

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
                    await this.RouteInputAsync(brokeredMessage, this.appContext, default).PreserveThreadContext();
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (this.initializationMonitor.IsNotStarted)
            {
                return;
            }

            this.FinalizeCore();
        }

        /// <summary>
        /// Routes the brokered message asynchronously, typically over the physical medium.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The send context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding an action to take further and an optional reply.
        /// </returns>
        protected override async Task<(RoutingInstruction action, IMessage reply)> RouteOutputAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            this.initializationMonitor.AssertIsCompletedSuccessfully();

            var serializedMessage = await this.serializationService.SerializeAsync(brokeredMessage, ctx => ctx.IncludeTypeInfo(true)).PreserveThreadContext();
            if (brokeredMessage.Recipients?.Any() ?? false)
            {
                foreach (var recipient in brokeredMessage.Recipients)
                {
                    var channelName = string.IsNullOrEmpty(recipient.AppInstanceId)
                                        ? string.IsNullOrEmpty(recipient.AppId)
                                            ? this.rootChannelName
                                            : $"{this.rootChannelName}:{recipient.AppId}"
                                        : $"{this.rootChannelName}:{recipient.AppInstanceId}";
                    await this.PublishAsync(serializedMessage, channelName, brokeredMessage.IsOneWay).PreserveThreadContext();
                }
            }
            else
            {
                await this.PublishAsync(serializedMessage, this.rootChannelName, brokeredMessage.IsOneWay).PreserveThreadContext();
            }

            return (RoutingInstruction.None, null);
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

        private void FinalizeCore()
        {
            if (!this.redisClient.IsInitialized)
            {
                return;
            }

            if (this.finalizationMonitor.IsCompleted)
            {
                return;
            }

            this.initializationMonitor.AssertIsCompletedSuccessfully();

            this.Logger.Info("Redis client is finalizing, stopping the Redis message router...");

            this.finalizationMonitor.Start();

            var messageRouterName = this.GetType().Name;
            try
            {
                this.messageQueue.Unsubscribe();
                this.appMessageQueue.Unsubscribe();
                this.appInstanceMessageQueue.Unsubscribe();

                this.finalizationMonitor.Complete();
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, $"{messageRouterName} failed to finalize.");
                this.finalizationMonitor.Fault(ex);
                throw;
            }
            finally
            {
                this.initializationMonitor.Reset();
            }
        }
    }
}