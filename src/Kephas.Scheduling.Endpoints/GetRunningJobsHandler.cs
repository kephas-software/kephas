// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetRunningJobsHandler.cs" company="Kephas Software SRL">
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
    /// Handler for <see cref="GetRunningJobsMessage"/>.
    /// </summary>
    public class GetRunningJobsHandler : MessageHandlerBase<GetRunningJobsMessage, GetRunningJobsResponseMessage>
    {
        private readonly IScheduler scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRunningJobsHandler"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        public GetRunningJobsHandler(IScheduler scheduler)
        {
            this.scheduler = scheduler;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The response promise.</returns>
        public override async Task<GetRunningJobsResponseMessage> ProcessAsync(GetRunningJobsMessage message, IMessagingContext context, CancellationToken token)
        {
            return new GetRunningJobsResponseMessage
            {
                Jobs = this.scheduler.GetRunningJobs()
                    .Select(jobResult => new RunningJobData
                    {
                        JobId = jobResult.JobId,
                        JobInfo = jobResult.JobInfo?.ToString(),
                        OperationState = jobResult.OperationState,
                        PercentCompleted = jobResult.PercentCompleted,
                        StartedAt = jobResult.StartedAt,
                        Elapsed = jobResult.Elapsed,
                    })
                    .ToArray(),
            };
        }
    }
}