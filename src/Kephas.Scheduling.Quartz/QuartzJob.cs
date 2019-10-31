// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuartzJob.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the quartz job class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz
{
    using System;
    using System.Threading.Tasks;

    using global::Quartz;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using Kephas.Workflow;

    /// <summary>
    /// A Quartz runtime job.
    /// </summary>
    [PersistJobDataAfterExecution]
    [AppServiceContract]
    public class QuartzJob : IJob
    {
        private readonly IWorkflowProcessor workflowProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuartzJob"/> class.
        /// </summary>
        /// <param name="workflowProcessor">The workflow processor.</param>
        public QuartzJob(IWorkflowProcessor workflowProcessor)
        {
            Requires.NotNull(workflowProcessor, nameof(workflowProcessor));

            this.workflowProcessor = workflowProcessor;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.ITrigger" />
        /// fires that is associated with the <see cref="T:Quartz.IJob" />.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public async Task Execute(IJobExecutionContext context)
        {
            var activity = this.GetActivity(context);
            var workflowConfig = this.GetWorkflowConfig(context, activity);
            var result = await this.workflowProcessor.ExecuteAsync(
                             activity,
                             activity.Target,
                             activity.Arguments,
                             workflowConfig,
                             context.CancellationToken).PreserveThreadContext();
            context.Result = result;
        }

        private IActivity GetActivity(IJobExecutionContext context)
        {
            // TODO
            throw new System.NotImplementedException();
        }

        private Action<IActivityContext> GetWorkflowConfig(IJobExecutionContext context, IActivity activity)
        {
            // TODO
            return ctx => ctx.Activity(activity);
        }
    }
}