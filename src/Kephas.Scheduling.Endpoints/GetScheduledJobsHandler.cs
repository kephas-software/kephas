// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetScheduledJobsHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

namespace Kephas.Scheduling.Endpoints
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Messaging;

    /// <summary>
    /// Message handler for <see cref="GetScheduledJobsMessage"/>.
    /// </summary>
    public class GetScheduledJobsHandler : MessageHandlerBase<GetScheduledJobsMessage, GetScheduledJobsResponseMessage>
    {
        private readonly IScheduler scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetScheduledJobsHandler"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        public GetScheduledJobsHandler(IScheduler scheduler)
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
        public override async Task<GetScheduledJobsResponseMessage> ProcessAsync(GetScheduledJobsMessage message, IMessagingContext context, CancellationToken token)
        {
            return new GetScheduledJobsResponseMessage
            {
                Jobs = this.scheduler.GetScheduledJobs()
                    .Select(jobInfo => new ScheduledJobData
                    {
                        Name = jobInfo.Name,
                        Triggers = jobInfo.Triggers.Select(t => t.ToString()).ToArray(),
                    })
                    .ToArray(),
            };
        }
    }
}