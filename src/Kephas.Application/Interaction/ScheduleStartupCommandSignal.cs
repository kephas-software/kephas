﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduleStartupCommandSignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Interaction
{
    using Kephas.Interaction;

    /// <summary>
    /// Signal for scheduling a startup command.
    /// </summary>
    public class ScheduleStartupCommandSignal : SignalBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleStartupCommandSignal"/> class.
        /// </summary>
        /// <param name="command">The command to be scheduled.</param>
        /// <param name="appId">The ID of the app for which the startup command should be added.</param>
        public ScheduleStartupCommandSignal(object command, string? appId = null)
            : base("Schedule command.")
        {
            this.Command = command;
            this.AppId = appId;
        }

        /// <summary>
        /// The ID of the app for which the startup command should be added.
        /// </summary>
        public string? AppId { get; }

        /// <summary>
        /// The command to be scheduled.
        /// </summary>
        public object Command { get; }
    }
}