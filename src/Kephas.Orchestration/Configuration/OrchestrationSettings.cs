// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchestrationSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Configuration
{
    using System;

    /// <summary>
    /// Settings for orchestration.
    /// </summary>
    public class OrchestrationSettings
    {
        /// <summary>
        /// Gets or sets the heartbeat interval.
        /// </summary>
        public TimeSpan HeartbeatInterval { get; set; } = TimeSpan.FromMinutes(1);
    }
}