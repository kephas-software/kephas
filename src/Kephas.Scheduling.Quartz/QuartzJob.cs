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
    using System.Threading.Tasks;

    using global::Quartz;

    using Kephas.Composition;
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
        private readonly ICompositionContext compositionContext;

        private readonly IWorkflowProcessor workflowProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuartzJob"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        /// <param name="workflowProcessor">The workflow processor.</param>
        public QuartzJob(ICompositionContext compositionContext, IWorkflowProcessor workflowProcessor)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));
            Requires.NotNull(workflowProcessor, nameof(workflowProcessor));

            this.compositionContext = compositionContext;
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
            var activityContext = this.GetActivityContext(context, activity);
            var result = await this.workflowProcessor.ExecuteAsync(
                             activity,
                             activity.Target,
                             activity.Arguments,
                             activityContext,
                             context.CancellationToken).PreserveThreadContext();
            context.Result = result;
        }

        private IActivity GetActivity(IJobExecutionContext context)
        {
            // TODO
            throw new System.NotImplementedException();
        }

        private IActivityContext GetActivityContext(IJobExecutionContext context, IActivity activity)
        {
            // TODO
            return new ActivityContext(this.compositionContext, this.workflowProcessor) { Activity = activity };
        }
    }
}