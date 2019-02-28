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
    using global::Quartz;

    using Kephas.Data.Model.AttributedModel;

    /// <summary>
    /// Interface for job detail.
    /// </summary>
    [NaturalKey(new[] { nameof(InstanceName), nameof(Group), nameof(Name) })]
    public interface IJobDetail : IGroupedEntityBase, INamedEntityBase
    {
    }

    /// <summary>
    /// A job detail extensions.
    /// </summary>
    public static class JobDetailExtensions
    {
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