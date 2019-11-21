// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRedisConnectionFactory.cs" company="Kephas Software SRL">
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
    /// Application service contract for creating Redis connections.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IRedisConnectionFactory
    {
        /// <summary>
        /// Gets a value indicating whether the factory is initialized or not.
        /// </summary>
        /// <value>
        /// True if the factory is initialized, false if not.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        /// Creates a Redis connection. It is the caller responsibility to properly dispose the created connection.
        /// </summary>
        /// <returns>
        /// The Redis connection.
        /// </returns>
        ConnectionMultiplexer CreateConnection();
    }
}
