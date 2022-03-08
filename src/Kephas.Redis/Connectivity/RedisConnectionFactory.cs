// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisConnectionFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Redis.Connectivity;

using System.Collections.Concurrent;
using System.Text;
using System.Web;

using Kephas.Application;
using Kephas.Connectivity;
using Kephas.Connectivity.AttributedModel;
using Kephas.Cryptography;
using Kephas.ExceptionHandling;
using Kephas.Interaction;
using Kephas.Logging;
using Kephas.Redis.Interaction;
using Kephas.Redis.Logging;
using Kephas.Security.Authentication;
using Kephas.Services;
using Kephas.Services.Transitions;
using Kephas.Threading.Tasks;
using StackExchange.Redis;

/// <summary>
/// Connection factory for <see cref="IRedisConnection"/>.
/// </summary>
/// <seealso cref="Loggable" />
/// <seealso cref="IConnectionFactory" />
/// <seealso cref="IAsyncInitializable" />
/// <seealso cref="IAsyncFinalizable" />
[ConnectionKind(ConnectionKind)]
[OverridePriority(Priority.Low)]
public class RedisConnectionFactory : Loggable, IConnectionFactory, IAsyncInitializable, IAsyncFinalizable
{
    /// <summary>
    /// The 'redis' connection kind.
    /// </summary>
    public const string ConnectionKind = "redis";

    private static int connectionCounter;

    private readonly InitializationMonitor<IConnectionFactory> initMonitor;
    private readonly FinalizationMonitor<IConnectionFactory> finMonitor;
    private readonly ILogManager logManager;
    private readonly IAppRuntime appRuntime;
    private readonly IEventHub eventHub;
    private readonly IEncryptionService encryptionService;

    private readonly ConcurrentDictionary<string, IConnectionMultiplexer> nativeConnections = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisConnectionFactory"/> class.
    /// </summary>
    /// <param name="appRuntime">The application runtime.</param>
    /// <param name="eventHub">The event hub.</param>
    /// <param name="encryptionService">The encryption service.</param>
    /// <param name="logManager">Optional. The log manager.</param>
    public RedisConnectionFactory(
        IAppRuntime appRuntime,
        IEventHub eventHub,
        IEncryptionService encryptionService,
        ILogManager? logManager = null)
        : base(logManager)
    {
        this.logManager = logManager ?? LoggingHelper.DefaultLogManager;
        this.appRuntime = appRuntime;
        this.eventHub = eventHub;
        this.encryptionService = encryptionService;
        this.initMonitor = new InitializationMonitor<IConnectionFactory>(this.GetType());
        this.finMonitor = new FinalizationMonitor<IConnectionFactory>(this.GetType());
    }

    /// <summary>
    /// Gets a value indicating whether the manager is initialized or not.
    /// </summary>
    /// <value>
    /// True if the manager is initialized, false if not.
    /// </value>
    public bool IsInitialized => this.initMonitor.IsCompletedSuccessfully;

    /// <summary>
    /// Creates the connection configured through the connection context.
    /// </summary>
    /// <param name="context">The connection creation context.</param>
    /// <returns>The newly created connection.</returns>
    public IConnection CreateConnection(IConnectionContext context)
        => new RedisConnection(context, this.GetOrCreateNativeConnection(context), this.logManager);

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
            this.initMonitor.Complete();

            await this.eventHub.PublishAsync(new ConnectionManagerStartedSignal(), context, cancellationToken)
                .PreserveThreadContext();
        }
        catch (Exception ex)
        {
            this.initMonitor.Fault(ex);

            await this.eventHub.PublishAsync(
                new ConnectionManagerStartedSignal(ex.Message, SeverityLevel.Error),
                context,
                cancellationToken).PreserveThreadContext();
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
            await this.eventHub.PublishAsync(new ConnectionManagerStoppingSignal(), context, cancellationToken)
                .PreserveThreadContext();

            foreach (var (key, nativeConnection) in this.nativeConnections)
            {
                this.DisposeNativeConnection(nativeConnection);
            }

            this.finMonitor.Complete();
        }
        catch (Exception ex)
        {
            this.Logger.Fatal(ex, "Error while closing the Redis connection.");
            this.finMonitor.Fault(ex);

            await this.eventHub.PublishAsync(
                new ConnectionManagerStoppingSignal(ex.Message, SeverityLevel.Error),
                context,
                cancellationToken).PreserveThreadContext();
        }
        finally
        {
            this.initMonitor.Reset();

            await this.eventHub.PublishAsync(
                new ConnectionManagerStoppedSignal(),
                context,
                cancellationToken).PreserveThreadContext();
        }
    }

    /// <summary>
    /// Disposes a previously created native connection.
    /// </summary>
    /// <param name="connection">The connection.</param>
    protected virtual void DisposeNativeConnection(IConnectionMultiplexer connection)
    {
        var clientName = connection.ClientName;
        connection.Close(allowCommandsToComplete: true);
        connection.Dispose();

        this.Logger.Debug("Redis connection '{redisConnection}' disposed.", clientName);
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
    /// <param name="context">The connection context.</param>
    /// <returns>
    /// The new connection.
    /// </returns>
    protected virtual IConnectionMultiplexer GetOrCreateNativeConnection(IConnectionContext context)
    {
        var host = context.Host ?? throw new ArgumentNullException(nameof(context.Host));
        var (connectionKey, options) = this.GetConfigurationOptions(host, context.Credentials);

        try
        {
            return this.nativeConnections.GetOrAdd(connectionKey, _ =>
            {
                var connection = ConnectionMultiplexer.Connect(options, this.CreateRedisLogger(context));
                connection.ConnectionFailed += this.HandleConnectionFailed;
                connection.ConnectionRestored += this.HandleConnectionRestored;
                connection.InternalError += this.HandleInternalError;

                this.Logger.Debug("Redis connection '{redisConnection}' to '{redisHost}' created.", connection.ClientName, host.Authority);
                return connection;
            });
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex, "Could not connect to Redis server {redisHost}.", host.Authority);
            throw;
        }
    }

    /// <summary>
    /// Gets configuration options.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>
    /// The configuration options together with the connection key.
    /// </returns>
    protected virtual (string key, ConfigurationOptions options) GetConfigurationOptions(string connectionString)
    {
        var configuration = ConfigurationOptions.Parse(connectionString);
        var connectionId = Interlocked.Increment(ref connectionCounter);
        configuration.ClientName = $"{this.appRuntime.GetAppInstanceId()}-{connectionId}";
        return (connectionString, configuration);
    }

    /// <summary>
    /// Gets configuration options.
    /// </summary>
    /// <param name="host">The host URI.</param>
    /// <param name="credentials">Optional. The token credentials.</param>
    /// <returns>
    /// The configuration options.
    /// </returns>
    protected virtual (string connectionKey, ConfigurationOptions options) GetConfigurationOptions(
        Uri host,
        ICredentials? credentials = null)
    {
        var connectionString = this.GetConnectionString(host);

        var (connectionKey, options) = this.GetConfigurationOptions(connectionString);

        switch (credentials)
        {
            case ITokenCredentials tokenProvider:
                options.Password = tokenProvider.Token;
                break;
            case IUserClearTextPasswordCredentials passwordProvider:
                options.User = passwordProvider.UserName;
                options.Password = passwordProvider.ClearTextPassword;
                break;
            case IUserPasswordCredentials passwordProvider:
                options.User = passwordProvider.UserName;
                options.Password = this.encryptionService.Decrypt(passwordProvider.Password);
                break;
            case null:
                // ignore
                break;
            default:
                throw new InvalidOperationException($"Credentials '{credentials.GetType()}' are not supported.");
        }

        return (connectionKey, options);
    }

    /// <summary>
    /// Gets the connection string out of the host URI.
    /// </summary>
    /// <param name="host">The host URI.</param>
    /// <returns>The connection string.</returns>
    protected virtual string GetConnectionString(Uri host)
    {
        var connectionBuilder = new StringBuilder();
        connectionBuilder.Append(host.Authority);
        if (string.IsNullOrWhiteSpace(host.Query))
        {
            return connectionBuilder.ToString();
        }

        var queryArgs = HttpUtility.ParseQueryString(host.Query);
        foreach (string key in queryArgs)
        {
            var values = queryArgs.GetValues(key);
            if (values == null)
            {
                connectionBuilder.Append(',').Append(key);
            }
            else
            {
                // last value wins.
                connectionBuilder.Append(',').Append(key).Append('=').Append(values[^1]);
            }
        }

        return connectionBuilder.ToString();
    }

    /// <summary>
    /// Handles the internal error event.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="eventArgs">Internal error event information.</param>
    protected virtual void HandleInternalError(object? sender, InternalErrorEventArgs eventArgs)
    {
        var connection = sender as IConnectionMultiplexer;
        this.Logger.Warn(eventArgs.Exception, "Redis connection '{redisConnection}' internal error.", connection?.ClientName);
    }

    /// <summary>
    /// Handles the connection failed event.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="eventArgs">Connection failed event information.</param>
    protected virtual void HandleConnectionFailed(object? sender, ConnectionFailedEventArgs eventArgs)
    {
        var connection = sender as IConnectionMultiplexer;
        this.Logger.Warn("Redis connection '{redisConnection}' failed.", connection?.ClientName);
    }

    /// <summary>
    /// Handles the connection restored event.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="eventArgs">Connection failed event information.</param>
    protected virtual void HandleConnectionRestored(object? sender, ConnectionFailedEventArgs eventArgs)
    {
        var connection = sender as IConnectionMultiplexer;
        this.Logger.Warn("Redis connection '{redisConnection}' restored.", connection?.ClientName);
    }
}