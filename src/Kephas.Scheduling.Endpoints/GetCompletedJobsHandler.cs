// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCompletedJobsHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Endpoints
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Formatting;
    using Kephas.Messaging;
    using Kephas.Operations;
    using Kephas.Scheduling.Jobs;
    using Kephas.Services;

    /// <summary>
    /// Handler for the <see cref="GetCompletedJobsMessage"/>.
    /// </summary>
    public class GetCompletedJobsHandler : MessageHandlerBase<GetCompletedJobsMessage, GetCompletedJobsResponse>
    {
        private readonly IScheduler scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetCompletedJobsHandler"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        public GetCompletedJobsHandler(IScheduler scheduler)
        {
            this.scheduler = scheduler;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public override async Task<GetCompletedJobsResponse> ProcessAsync(GetCompletedJobsMessage message, IMessagingContext context, CancellationToken token)
        {
            await Task.Yield();

            var completedJobsQuery = this.scheduler.GetCompletedJobs(ctx => ctx.Impersonate(context));
            var totalCount = completedJobsQuery.Count();
            var jobsQuery = completedJobsQuery
                .OrderByDescending(j => j.StartedAt)
                .Skip(message.Skip)
                .Take(message.Take);

            return new GetCompletedJobsResponse
            {
                Jobs = jobsQuery
                    .Select(jobResult => new CompletedJobData
                    {
                        JobId = jobResult.RunningJobId,
                        ScheduledJob = jobResult.ScheduledJob.ToString(),
                        ScheduledJobId = jobResult.ScheduledJob.Id,
                        OperationState = jobResult.OperationState,
                        PercentCompleted = jobResult.PercentCompleted,
                        StartedAt = jobResult.StartedAt,
                        EndedAt = jobResult.EndedAt,
                        Elapsed = jobResult.Elapsed,
                        ReturnValue = this.GetJobReturnValue(jobResult, context),
                    })
                    .ToArray(),
                TotalCount = totalCount,
            };
        }

        /// <summary>
        /// Gets the return value of the job. Makes sure that the output value is serializable,
        /// optionally converting it to a DTO.
        /// </summary>
        /// <param name="jobResult">The job result.</param>
        /// <param name="messagingContext">The messaging context.</param>
        /// <returns>A serializable return value.</returns>
        protected virtual object? GetJobReturnValue(IJobResult jobResult, IMessagingContext messagingContext)
        {
            using var formattingContext = new Context(messagingContext);
            return jobResult.OperationState == OperationState.Completed && jobResult.Value != null
                ? jobResult.Value.ToData(formattingContext)
                : null;
        }
    }
}