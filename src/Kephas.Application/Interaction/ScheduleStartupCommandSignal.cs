// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduleStartupCommandSignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Interaction
{
    using Kephas.ExceptionHandling;
    using Kephas.Interaction;

    /// <summary>
    /// Signal for scheduling a startup command.
    /// </summary>
    public class ScheduleStartupCommandSignal : ISignal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleStartupCommandSignal"/> class.
        /// </summary>
        /// <param name="command">The command to be scheduled.</param>
        public ScheduleStartupCommandSignal(object command)
        {
            this.Command = command;
        }

        /// <summary>
        /// The command to be scheduled.
        /// </summary>
        public object Command { get; }

        /// <summary>
        /// The signal message.
        /// </summary>
        public string Message { get; } = "Schedule a command.";

        /// <summary>
        /// The severity.
        /// </summary>
        public SeverityLevel Severity { get; } = SeverityLevel.Info;
    }
}