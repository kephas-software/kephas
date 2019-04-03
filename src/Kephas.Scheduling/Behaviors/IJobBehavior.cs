// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJobBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IJobBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Behaviors
{
    using Kephas.Workflow.Behaviors;

    /// <summary>
    /// Base contract for controlling the execution of jobs.
    /// </summary>
    /// <typeparam name="TJob">Type of the job.</typeparam>
    public interface IJobBehavior<TJob> : IActivityBehavior<TJob>
        where TJob : IJob
    {
    }
}