// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DistributedMessagingSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the distributed messaging settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;

    /// <summary>
    /// A distributed messaging settings.
    /// </summary>
    public class DistributedMessagingSettings
    {
        /// <summary>
        /// Gets or sets the default timeout.
        /// </summary>
        /// <value>
        /// The default timeout.
        /// </value>
        public TimeSpan? DefaultTimeout { get; set; }
    }
}
