// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRedisConnectionManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default Redis connection manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Redis
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
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
    /// The default Redis connection manager.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultRedisConnectionManager : Loggable, IRedisConnectionManager, IAsyncInitializable, IAsyncFinalizable
    {
        private static int connectionCounter;

        private readonly InitializationMonitor<IRedisConnectionManager> initMonitor;
        private readonly FinalizationMonitor<IRedisConnectionManager> finMonitor;
        private readonly ILogManager logManager;
        private readonly IAppRuntime appRuntime;
        private readonly IConfiguration<RedisClientSettings> redisConfiguration;
        private readonly IEventHub eventHub;
        private IContext appContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRedisConnectionManager"/> class.
        /// </summary>
        /// <param name="logManager">Manager for log.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="redisConfiguration">The redis configuration.</param>
        /// <param name="eventHub">The event hub.</param>
        public DefaultRedisConnectionManager(
            ILogManager logManager,
            IAppRuntime appRuntime,
            IConfiguration<RedisClientSettings> redisConfiguration,
            IEventHub eventHub)
            : base(logManager)
        {
            this.logManager = logManager;
            this.appRuntime = appRuntime;
            this.redisConfiguration = redisConfiguration;
            this.eventHub = eventHub;
            this.initMonitor = new InitializationMonitor<IRedisConnectionManager>(this.GetType());
            this.finMonitor = new FinalizationMonitor<IRedisConnectionManager>(this.GetType());
        }

        /// <summary>
        /// Gets a value indicating whether the manager is initialized or not.
        /// </summary>
        /// <value>
        /// True if the manager is initialized, false if not.
        /// </value>
        public bool IsInitialized => this.initMonitor.IsCompletedSuccessfully;

        /// <summary>
        /// Creates the connection based on <see cref="RedisClientSettings"/>.
        /// </summary>
        /// <remarks>
        /// It is the caller responsibility to properly dispose the created connection.
        /// Use <see cref="DisposeConnection(IConnectionMultiplexer)"/> method to properly dispose of the created connection.
        /// </remarks>
        /// <returns>
        /// The new connection.
        /// </returns>
        public IConnectionMultiplexer CreateConnection()
        {
            this.initMonitor.AssertIsCompletedSuccessfully();

            return this.CreateConnectionCore(this.appContext);
        }

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">Optional. An optional context for initialization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        public async Task InitializeAsync(IContext? context = null, CancellationToken cancellationToken = default)
        {
            this.initMonitor.AssertIsNotStarted();

            this.initMonitor.Start();

            try
            {
                var settings = this.redisConfiguration.Settings;
                this.appContext = context;
                using (var connection = this.CreateConnectionCore(context))
                {
                    this.Logger.Info("Successfully connected to Redis.");
                }

                this.initMonitor.Complete();

                await this.eventHub.PublishAsync(new ConnectionManagerStartedSignal(), context, cancellationToken).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(ex, "Error while connecting to Redis.");
                this.initMonitor.Fault(ex);

                await this.eventHub.PublishAsync(new ConnectionManagerStartedSignal(ex.Message, SeverityLevel.Error), context, cancellationToken).PreserveThreadContext();
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
        public async Task FinalizeAsync(IContext? context = null, CancellationToken cancellationToken = default)
        {
            this.finMonitor.AssertIsNotStarted();

            this.finMonitor.Start();

            try
            {
                await this.eventHub.PublishAsync(new ConnectionManagerStoppingSignal(), context, cancellationToken).PreserveThreadContext();

                this.finMonitor.Complete();
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(ex, "Error while closing the Redis connection.");
                this.finMonitor.Fault(ex);

                await this.eventHub.PublishAsync(new ConnectionManagerStoppingSignal(ex.Message, SeverityLevel.Error), context, cancellationToken).PreserveThreadContext();
            }
            finally
            {
                this.initMonitor.Reset();
            }
        }

        /// <summary>
        /// Disposes a previously created connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public virtual void DisposeConnection(IConnectionMultiplexer connection)
        {
            if (connection != null)
            {
                var clientName = connection.ClientName;
                connection.Close(allowCommandsToComplete: true);
                connection.Dispose();

                this.Logger.Debug("Redis connection '{redisConnection}' disposed.", clientName);
            }
        }

        /// <summary>
        /// Creates a Redis logger.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <returns>
        /// The new Redis logger.
        /// </returns>
        protected virtual TextWriter CreateRedisLogger(IContext? context)
        {
            return new RedisLogger(this.logManager);
        }

        /// <summary>
        /// Creates the connection. This method may be overridden.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <returns>
        /// The new connection.
        /// </returns>
        protected virtual IConnectionMultiplexer CreateConnectionCore(IContext? context)
        {
            var connection = ConnectionMultiplexer.Connect(this.GetConfigurationOptions(context), this.CreateRedisLogger(context));
            connection.ConnectionFailed += this.HandleConnectionFailed;
            connection.ConnectionRestored += this.HandleConnectionRestored;
            connection.InternalError += this.HandleInternalError;

            this.Logger.Debug("Redis connection '{redisConnection}' created.", connection.ClientName);

            return connection;
        }

        /// <summary>
        /// Gets configuration options.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <returns>
        /// The configuration options.
        /// </returns>
        protected virtual ConfigurationOptions GetConfigurationOptions(IContext? context)
        {
            var settings = this.redisConfiguration.Settings;
            var configuration = ConfigurationOptions.Parse(settings.ConnectionString);
            var connectionId = Interlocked.Increment(ref connectionCounter);
            configuration.ClientName = $"{this.appRuntime.GetAppInstanceId()}-{connectionId}";
            return configuration;
        }

        /// <summary>
        /// Handles the internal error event.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="eventArgs">Internal error event information.</param>
        protected virtual void HandleInternalError(object sender, InternalErrorEventArgs eventArgs)
        {
            var connection = sender as IConnectionMultiplexer;
            this.Logger.Warn(eventArgs.Exception, "Redis connection '{redisConnection}' internal error.", connection?.ClientName);
        }

        /// <summary>
        /// Handles the connection failed event.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="eventArgs">Connection failed event information.</param>
        protected virtual void HandleConnectionFailed(object sender, ConnectionFailedEventArgs eventArgs)
        {
            var connection = sender as IConnectionMultiplexer;
            this.Logger.Warn("Redis connection '{redisConnection}' failed.", connection?.ClientName);
        }

        /// <summary>
        /// Handles the connection restored event.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="eventArgs">Connection failed event information.</param>
        protected virtual void HandleConnectionRestored(object sender, ConnectionFailedEventArgs eventArgs)
        {
            var connection = sender as IConnectionMultiplexer;
            this.Logger.Warn("Redis connection '{redisConnection}' restored.", connection?.ClientName);
        }
    }
}
