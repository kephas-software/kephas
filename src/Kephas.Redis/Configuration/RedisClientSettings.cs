// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisClientSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the redis client settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Redis.Configuration
{
    using Kephas.Configuration;

    /// <summary>
    /// The Redis client settings.
    /// </summary>
    public class RedisClientSettings : SettingsBase, ISettings
    {
        /// <summary>
        /// Gets or sets the Redis connection string.
        /// </summary>
        /// <value>
        /// The Redis connection string.
        /// </value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the application namespace.
        /// </summary>
        /// <value>
        /// The application namespace.
        /// </value>
        public string? Namespace { get; set; }
    }
}
