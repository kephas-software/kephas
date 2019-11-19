// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRedisClient.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default redis client class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Redis
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Configuration;
    using Kephas.ExceptionHandling;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Redis.Configuration;
    using Kephas.Redis.Interaction;
    using Kephas.Redis.Logging;
    using Kephas.Services;
    using Kephas.Services.Transitions;
    using Kephas.Threading.Tasks;
    using StackExchange.Redis;

    /// <summary>
    /// A default redis client.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultRedisClient : Loggable, IRedisClient, IAsyncInitializable, IAsyncFinalizable
    {
        private readonly InitializationMonitor<IRedisClient> initMonitor;
        private readonly FinalizationMonitor<IRedisClient> finMonitor;
        private readonly ILogManager logManager;
        private readonly IConfiguration<RedisClientSettings> redisConfiguration;
        private readonly IEventHub eventHub;
        private ConnectionMultiplexer connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRedisClient"/> class.
        /// </summary>
        /// <param name="logManager">Manager for log.</param>
        /// <param name="redisConfiguration">The redis configuration.</param>
        /// <param name="eventHub">The event hub.</param>
        public DefaultRedisClient(
            ILogManager logManager,
            IConfiguration<RedisClientSettings> redisConfiguration,
            IEventHub eventHub)
        {
            this.logManager = logManager;
            this.redisConfiguration = redisConfiguration;
            this.eventHub = eventHub;
            this.initMonitor = new InitializationMonitor<IRedisClient>(this.GetType());
            this.finMonitor = new FinalizationMonitor<IRedisClient>(this.GetType());
        }

        /// <summary>
        /// Gets a value indicating whether the Redis client is initialized.
        /// </summary>
        /// <value>
        /// True if the Redis client is initialized, false if not.
        /// </value>
        public bool IsInitialized => this.initMonitor.IsCompletedSuccessfully;

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns>
        /// The connection.
        /// </returns>
        public ConnectionMultiplexer GetConnection()
        {
            this.initMonitor.AssertIsCompletedSuccessfully();
            return this.connection;
        }

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">Optional. An optional context for initialization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        public async Task InitializeAsync(IContext context = null, CancellationToken cancellationToken = default)
        {
            this.initMonitor.AssertIsNotStarted();

            this.initMonitor.Start();

            try
            {
                var settings = this.redisConfiguration.Settings;
                this.connection = ConnectionMultiplexer.Connect(settings.ConnectionString, this.CreateRedisLogger(context));
                this.initMonitor.Complete();

                await this.eventHub.PublishAsync(new RedisClientStartedSignal(), context, cancellationToken).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(ex, "Error while connecting to Redis server.");
                this.initMonitor.Fault(ex);

                await this.eventHub.PublishAsync(new RedisClientStartedSignal(ex.Message, SeverityLevel.Error), context, cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <param name="context">Optional. An optional context for finalization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public async Task FinalizeAsync(IContext context = null, CancellationToken cancellationToken = default)
        {
            this.finMonitor.AssertIsNotStarted();

            this.finMonitor.Start();

            try
            {
                await this.eventHub.PublishAsync(new RedisClientStoppingSignal(), context, cancellationToken).PreserveThreadContext();

                if (this.connection != null)
                {
                    await this.connection.CloseAsync(allowCommandsToComplete: true).PreserveThreadContext();
                    this.connection.Dispose();
                }

                this.finMonitor.Complete();
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(ex, "Error while closing the Redis connection.");
                this.finMonitor.Fault(ex);

                await this.eventHub.PublishAsync(new RedisClientStoppingSignal(ex.Message, SeverityLevel.Error), context, cancellationToken).PreserveThreadContext();
            }
            finally
            {
                this.initMonitor.Reset();
            }
        }

        /// <summary>
        /// Creates a Redis logger.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <returns>
        /// The new Redis logger.
        /// </returns>
        protected virtual TextWriter CreateRedisLogger(IContext context)
        {
            return new RedisLogger(this.logManager);
        }
    }
}
