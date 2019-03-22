// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultConfigurationStore.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default configuration store class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System;
    using System.Collections.Concurrent;

    using Kephas.Dynamic;

    /// <summary>
    /// A default configuration store.
    /// </summary>
    public class DefaultConfigurationStore : Expando, IConfigurationStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConfigurationStore"/> class.
        /// </summary>
        public DefaultConfigurationStore()
            : base(new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase))
        {
        }
    }
}