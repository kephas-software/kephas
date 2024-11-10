// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisConnection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Redis.Connectivity;

using Kephas.Connectivity;
using Kephas.Logging;
using StackExchange.Redis;

/// <summary>
/// Implementation of <see cref="IRedisConnection"/>, an <see cref="IConnection"/> for Redis.
/// </summary>
public class RedisConnection : Loggable, IRedisConnection
{
    private readonly IConnectionContext connectionContext;
    private readonly IConnectionMultiplexer nativeConnection;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisConnection"/> class.
    /// </summary>
    /// <param name="connectionContext">The connection context.</param>
    /// <param name="nativeConnection">The native connection.</param>
    /// <param name="logManager">Optional. The log manager.</param>
    protected internal RedisConnection(
        IConnectionContext connectionContext,
        IConnectionMultiplexer nativeConnection,
        ILogManager? logManager = null)
        : base(logManager)
    {
        this.connectionContext = connectionContext;
        this.nativeConnection = nativeConnection;
    }

    /// <summary>
    /// Gets the object the current instance adapts.
    /// </summary>
    /// <value>
    /// The object the current instance adapts.
    /// </value>
    IConnectionMultiplexer IAdapter<IConnectionMultiplexer>.Of => this.nativeConnection;

    /// <summary>
    /// Opens the connection asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The asynchronous result.</returns>
    public Task OpenAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.connectionContext.Dispose();

            // do not dispose the connection multiplexer, as it will be reused for other connections.
        }
    }
}