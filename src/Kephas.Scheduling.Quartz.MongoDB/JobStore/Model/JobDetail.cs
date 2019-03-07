// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobDetail.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the job detail class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.MongoDB.JobStore.Model
{
    using System;

    using global::Quartz;

    using Kephas.Activation;
    using Kephas.Scheduling.Quartz.JobStore.Model;

    using IJobDetail = global::Kephas.Scheduling.Quartz.JobStore.Model.IJobDetail;

    /// <summary>
    /// Implementation of a <see cref="IJobDetail"/> entity.
    /// </summary>
    [ImplementationFor(typeof(IJobDetail))]
    public class JobDetail : GroupedQuartzEntityBase, IJobDetail
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of the job.
        /// </summary>
        /// <value>
        /// The type of the job.
        /// </value>
        public Type JobType { get; set; }

        /// <summary>
        /// Gets or sets the job data map.
        /// </summary>
        /// <value>
        /// The job data map.
        /// </value>
        public JobDataMap JobDataMap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object is durable.
        /// </summary>
        /// <value>
        /// True if durable, false if not.
        /// </value>
        public bool Durable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the persist job data after execution.
        /// </summary>
        /// <value>
        /// True if persist job data after execution, false if not.
        /// </value>
        public bool PersistJobDataAfterExecution { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the concurrent execution disallowed.
        /// </summary>
        /// <value>
        /// True if concurrent execution disallowed, false if not.
        /// </value>
        public bool ConcurrentExecutionDisallowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the requests recovery.
        /// </summary>
        /// <value>
        /// True if requests recovery, false if not.
        /// </value>
        public bool RequestsRecovery { get; set; }
    }
}