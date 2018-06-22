// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppHeartbeatEvent.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application heartbeat event class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Endpoints
{
    using System;

    /// <summary>
    /// An application heartbeat event.
    /// </summary>
    public class AppHeartbeatEvent : IAppEvent
    {
        /// <summary>
        /// Gets or sets information describing the application.
        /// </summary>
        /// <value>
        /// Information describing the application.
        /// </value>
        public IAppInfo AppInfo { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTimeOffset Timestamp { get; set; }
    }
}