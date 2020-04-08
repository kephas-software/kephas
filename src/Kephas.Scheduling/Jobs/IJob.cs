// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJob.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IJob interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Jobs
{
    using System;

    using Kephas.Scheduling.Reflection;
    using Kephas.Workflow;

    /// <summary>
    /// Contract for automated pieces of work that can be performed at either a particular time, or on a recurring schedule.
    /// </summary>
    /// <remarks>Jobs are specializations of activities.</remarks>
    public interface IJob : IActivity, IDisposable
    {
        /// <summary>
        /// Gets the type information for this instance.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        new IJobInfo GetTypeInfo();
    }
}