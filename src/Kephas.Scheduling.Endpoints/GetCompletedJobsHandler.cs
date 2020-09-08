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

    using Kephas.Messaging;

    /// <summary>
    /// Handler for the <see cref="GetCompletedJobsMessage"/>.
    /// </summary>
    public class GetCompletedJobsHandler : MessageHandlerBase<GetCompletedJobsMessage, GetCompletedJobsResponseMessage>
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
        public override async Task<GetCompletedJobsResponseMessage> ProcessAsync(GetCompletedJobsMessage message, IMessagingContext context, CancellationToken token)
        {
            await Task.Yield();

            var completedJobsQuery = this.scheduler.GetCompletedJobs();
            var totalCount = completedJobsQuery.Count();
            var jobsQuery = completedJobsQuery
                .OrderByDescending(j => j.StartedAt)
                .Skip(message.Skip)
                .Take(message.Take);

            return new GetCompletedJobsResponseMessage
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
                    })
                    .ToArray(),
                TotalCount = totalCount,
            };
        }
    }
}