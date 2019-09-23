// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppStartedEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application started event class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Endpoints
{
    using System;

    /// <summary>
    /// An application started event.
    /// </summary>
    public class AppStartedEvent : IAppEvent
    {
        /// <summary>
        /// Gets or sets runtime information describing the application.
        /// </summary>
        /// <value>
        /// Runtime information describing the application.
        /// </value>
        public IRuntimeAppInfo AppInfo { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    }
}