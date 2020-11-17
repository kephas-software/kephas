// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchestrationSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Configuration
{
    using System;

    using Kephas.Configuration;
    using Kephas.Security.Authorization;
    using Kephas.Security.Authorization.AttributedModel;

    /// <summary>
    /// Settings for orchestration.
    /// </summary>
    [RequiresPermission(typeof(AppAdminPermission))]
    public class OrchestrationSettings : SettingsBase, ISettings
    {
        /// <summary>
        /// Gets or sets the heartbeat interval.
        /// </summary>
        public TimeSpan HeartbeatInterval { get; set; } = TimeSpan.FromMinutes(1);
    }
}