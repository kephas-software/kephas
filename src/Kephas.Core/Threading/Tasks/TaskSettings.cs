// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the task settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading.Tasks
{
    using System;

    using Kephas.Configuration;

    /// <summary>
    /// Task settings.
    /// </summary>
    public class TaskSettings : SettingsBase
    {
        /// <summary>
        /// Gets or sets the default value of milliseconds to wait a task in a completion check cycle when simulating synchronous calls.
        /// The default value is 20 milliseconds.
        /// </summary>
        public int? DefaultWaitMilliseconds { get; set; } = 20;

        /// <summary>
        /// Gets or sets the default timeout when waiting for task completion in simulating synchronous calls.
        /// The default value is 30 seconds.
        /// </summary>
        public TimeSpan? DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
