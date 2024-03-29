﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppStoppedEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application stopped event class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Interaction
{
    using System;

    /// <summary>
    /// An application stopped event.
    /// </summary>
    public class AppStoppedEvent : IAppEvent
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