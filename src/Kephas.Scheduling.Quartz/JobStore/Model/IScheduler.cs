// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScheduler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IScheduler interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Model
{
    using System;

    using Kephas.Data.Model.AttributedModel;

    /// <summary>
    /// Values that represent scheduler states.
    /// </summary>
    public enum SchedulerState
    {
        Started,
        Running,
        Paused,
        Resumed
    }

    /// <summary>
    /// Interface for scheduler.
    /// </summary>
    [NaturalKey(new[] { nameof(InstanceName), nameof(InstanceId) })]
    public interface IScheduler : IEntityBase
    {
        /// <summary>
        /// Gets or sets the identifier of the scheduler.
        /// </summary>
        /// <value>
        /// The identifier of the scheduler.
        /// </value>
        string InstanceId { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        SchedulerState State { get; set; }

        /// <summary>
        /// Gets or sets the Date/Time of the last check in.
        /// </summary>
        /// <value>
        /// The last check in.
        /// </value>
        DateTime? LastCheckIn { get; set; }
    }
}