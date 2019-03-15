// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISchedulingJobStoreContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISchedulingJobStoreContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore
{
    using System;

    using Kephas.Data;
    using Kephas.Services;

    /// <summary>
    /// Interface for scheduling job store context.
    /// </summary>
    public interface ISchedulingJobStoreContext : IContext
    {
        /// <summary>
        /// Gets or sets the data context factory.
        /// </summary>
        /// <value>
        /// The data context factory.
        /// </value>
        Func<IContext, IDataContext> DataContextFactory { get; set; }
    }
}