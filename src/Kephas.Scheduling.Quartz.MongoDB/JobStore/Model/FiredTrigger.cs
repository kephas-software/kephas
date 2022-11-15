// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FiredTrigger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the fired trigger class.
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
    /// Implementation of a <see cref="IFiredTrigger"/> entity.
    /// </summary>
    [ImplementationFor(typeof(IFiredTrigger))]
    public class FiredTrigger : QuartzEntityBase, IFiredTrigger
    {
        /// <summary>
        /// Gets or sets the identifier of the fired instance.
        /// </summary>
        /// <value>
        /// The identifier of the fired instance.
        /// </value>
        public string FiredInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the instance.
        /// </summary>
        /// <value>
        /// The identifier of the instance.
        /// </value>
        public string InstanceId { get; set; }

        /// <summary>
        /// Gets or sets the name of the trigger.
        /// </summary>
        /// <value>
        /// The name of the trigger.
        /// </value>
        public string TriggerName { get; set; }

        /// <summary>
        /// Gets or sets the group the trigger belongs to.
        /// </summary>
        /// <value>
        /// The trigger group.
        /// </value>
        public string TriggerGroup { get; set; }

        /// <summary>
        /// Gets or sets the name of the job.
        /// </summary>
        /// <value>
        /// The name of the job.
        /// </value>
        public string JobName { get; set; }

        /// <summary>
        /// Gets or sets the group the job belongs to.
        /// </summary>
        /// <value>
        /// The job group.
        /// </value>
        public string JobGroup { get; set; }

        /// <summary>
        /// Gets or sets the Date/Time of the fired time.
        /// </summary>
        /// <value>
        /// The fired time.
        /// </value>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Fired { get; set; }

        /// <summary>
        /// Gets or sets the Date/Time of the scheduled time.
        /// </summary>
        /// <value>
        /// The scheduled time.
        /// </value>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? Scheduled { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        [BsonRepresentation(BsonType.String)]
        public TriggerState State { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the concurrent execution is disallowed.
        /// </summary>
        /// <value>
        /// True if concurrent execution is disallowed, false if not.
        /// </value>
        public bool ConcurrentExecutionDisallowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether recovery is requested.
        /// </summary>
        /// <value>
        /// True if recovery is requested, false if not.
        /// </value>
        public bool RequestsRecovery { get; set; }
    }
}