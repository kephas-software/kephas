// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISchedulingJobStore.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISchedulingJobStore interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore
{
    using System;

    using global::Quartz.Spi;

    using Kephas.Data;
    using Kephas.Services;

    /// <summary>
    /// Interface for scheduling job store.
    /// </summary>
    [AppServiceContract]
    public interface ISchedulingJobStore : IJobStore, IInitializable
    {
        /// <summary>
        /// Gets or sets a value informing the <see cref="T:Quartz.Spi.IJobStore" /> of the Scheduler instance's Id,
        /// prior to initialize being invoked.
        /// </summary>
        new string InstanceId { get; set; }

        /// <summary>
        /// Gets or sets a value informing the <see cref="T:Quartz.Spi.IJobStore" /> of the Scheduler instance's name,
        /// prior to initialize being invoked.
        /// </summary>
        new string InstanceName { get; set; }

        /// <summary>
        /// Gets the data context factory.
        /// </summary>
        /// <value>
        /// The data context factory.
        /// </value>
        Func<IContext, IDataContext> DataContextFactory { get; }
    }
}