// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scheduler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scheduler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.MongoDB.JobStore.Model
{
    using System;

    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization.Attributes;

    using Kephas.Activation;
    using Kephas.Scheduling.Quartz.JobStore.Model;

    /// <summary>
    /// A scheduler.
    /// </summary>
    [ImplementationFor(typeof(IScheduler))]
    public class Scheduler : QuartzEntityBase, IScheduler
    {
        /// <summary>
        /// Gets or sets the identifier of the scheduler.
        /// </summary>
        /// <value>
        /// The identifier of the scheduler.
        /// </value>
        public string InstanceId { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        [BsonRepresentation(BsonType.String)]
        public SchedulerState State { get; set; }

        /// <summary>
        /// Gets or sets the Date/Time of the last check in.
        /// </summary>
        /// <value>
        /// The last check in.
        /// </value>
        public DateTime? LastCheckIn { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{typeof(Scheduler).Name} {this.InstanceId}/{this.InstanceName}";
        }
    }
}