// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRedisClient.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IRedisClient interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Redis
{
    using Kephas.Services;
    using StackExchange.Redis;

    /// <summary>
    /// Application service contract for the singleton Redis client.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IRedisClient
    {
        /// <summary>
        /// Gets a value indicating whether the Redis client is initialized.
        /// </summary>
        /// <value>
        /// True if the Redis client is initialized, false if not.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        /// Gets the Redis connection.
        /// </summary>
        /// <returns>
        /// The Redis connection.
        /// </returns>
        ConnectionMultiplexer GetConnection();
    }
}
