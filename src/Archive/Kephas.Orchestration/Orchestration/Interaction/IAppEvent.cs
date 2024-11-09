// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppEvent interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Interaction
{
    using System;

    using Kephas.Messaging.Events;

    /// <summary>
    /// Interface for application event.
    /// </summary>
    public interface IAppEvent
    {
        /// <summary>
        /// Gets or sets runtime information describing the application.
        /// </summary>
        /// <value>
        /// Runtime information describing the application.
        /// </value>
        IRuntimeAppInfo AppInfo { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        DateTimeOffset Timestamp { get; set; }
    }
}