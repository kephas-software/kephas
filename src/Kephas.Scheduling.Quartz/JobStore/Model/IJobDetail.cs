// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJobDetail.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IJobDetail interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Model
{
    using System;

    using global::Quartz;
    using global::Quartz.Impl;

    using Kephas.Data.Model.AttributedModel;

    /// <summary>
    /// Interface for job detail.
    /// </summary>
    [NaturalKey(new[] { nameof(InstanceName), nameof(Group), nameof(Name) })]
    public interface IJobDetail : IGroupedEntityBase, INamedEntityBase
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of the job.
        /// </summary>
        /// <value>
        /// The type of the job.
        /// </value>
        Type JobType { get; set; }

        /// <summary>
        /// Gets or sets the job data map.
        /// </summary>
        /// <value>
        /// The job data map.
        /// </value>
        JobDataMap JobDataMap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object is durable.
        /// </summary>
        /// <value>
        /// True if durable, false if not.
        /// </value>
        bool Durable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the persist job data after execution.
        /// </summary>
        /// <value>
        /// True if persist job data after execution, false if not.
        /// </value>
        bool PersistJobDataAfterExecution { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the concurrent execution disallowed.
        /// </summary>
        /// <value>
        /// True if concurrent execution disallowed, false if not.
        /// </value>
        bool ConcurrentExecutionDisallowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the requests recovery.
        /// </summary>
        /// <value>
        /// True if requests recovery, false if not.
        /// </value>
        bool RequestsRecovery { get; set; }
    }

    /// <summary>
    /// A job detail extensions.
    /// </summary>
    public static class JobDetailExtensions
    {
        /// <summary>
        /// An IJobDetail extension method that gets job detail.
        /// </summary>
        /// <param name="jobDetail">The jobDetail to act on.</param>
        /// <returns>
        /// The job detail.
        /// </returns>
        public static global::Quartz.IJobDetail GetJobDetail(this IJobDetail jobDetail)
        {
            // The missing properties are figured out at runtime from the job type attributes
            return new JobDetailImpl
                       {
                           Key = jobDetail.GetJobKey(),
                           Description = jobDetail.Description,
                           JobType = jobDetail.JobType,
                           JobDataMap = jobDetail.JobDataMap,
                           Durable = jobDetail.Durable,
                           RequestsRecovery = jobDetail.RequestsRecovery
                       };
        }

        /// <summary>
        /// An IJobDetail extension method that gets job key.
        /// </summary>
        /// <param name="jobDetail">The jobDetail to act on.</param>
        /// <returns>
        /// The job key.
        /// </returns>
        public static JobKey GetJobKey(this IJobDetail jobDetail)
        {
            return new JobKey(jobDetail.Name, jobDetail.Group);
        }
    }
}