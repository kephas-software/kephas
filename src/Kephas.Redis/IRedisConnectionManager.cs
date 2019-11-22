// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRedisConnectionManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IRedisConnectionManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Redis
{
    using Kephas.Redis.Configuration;
    using Kephas.Services;
    using StackExchange.Redis;

    /// <summary>
    /// Application service contract for managing Redis connections.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IRedisConnectionManager
    {
        /// <summary>
        /// Gets a value indicating whether the manager is initialized or not.
        /// </summary>
        /// <value>
        /// True if the manager is initialized, false if not.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        /// Creates the connection based on <see cref="RedisClientSettings"/>.
        /// </summary>
        /// <remarks>
        /// It is the caller responsibility to properly dispose the created connection.
        /// Use <see cref="DisposeConnection(IConnectionMultiplexer)"/> method to properly dispose of the created connection.
        /// </remarks>
        /// <returns>
        /// The Redis connection.
        /// </returns>
        IConnectionMultiplexer CreateConnection();

        /// <summary>
        /// Disposes a previously created connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        void DisposeConnection(IConnectionMultiplexer connection);
    }
}
