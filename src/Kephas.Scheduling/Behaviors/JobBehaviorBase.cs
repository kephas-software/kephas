// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobBehaviorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the job behavior base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Behaviors
{
    using Kephas.Logging;
    using Kephas.Scheduling.Jobs;
    using Kephas.Workflow.Behaviors;

    /// <summary>
    /// Base class for a job behavior.
    /// </summary>
    /// <typeparam name="TJob">Type of the job.</typeparam>
    public abstract class JobBehaviorBase<TJob> : ActivityBehaviorBase<TJob>, IJobBehavior<TJob>
        where TJob : IJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobBehaviorBase{TJob}"/> class.
        /// </summary>
        /// <param name="logManager">Optional. Manager for log.</param>
        public JobBehaviorBase(ILogManager? logManager = null)
            : base(logManager)
        {
        }
    }
}
