// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchestrationSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Configuration
{
    using System;
    using System.Collections.Generic;

    using Kephas.Application.Configuration;
    using Kephas.Dynamic;

    /// <summary>
    /// Settings for orchestration.
    /// </summary>
    public class OrchestrationSettings : Expando
    {
        /// <summary>
        /// Gets the settings for the application instances.
        /// </summary>
        public IDictionary<string, AppSettings> Instances { get; } = new Dictionary<string, AppSettings>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Gets or sets the commands to be executed upon startup, when the application is started for the first time.
        /// </summary>
        /// <remarks>
        /// The application will take care to remove the executed commands from this list once they were executed.
        /// </remarks>
        public object[]? SetupCommands { get; set; }

        /// <summary>
        /// Gets or sets the heartbeat interval.
        /// </summary>
        public TimeSpan HeartbeatInterval { get; set; } = TimeSpan.FromMinutes(1);
    }
}