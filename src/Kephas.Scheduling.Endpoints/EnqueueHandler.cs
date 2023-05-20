// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnqueueHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Endpoints
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Message handler for <see cref="EnqueueMessage"/>.
    /// </summary>
    public class EnqueueHandler : IMessageHandler<EnqueueMessage, EnqueueResponse>
    {
        private readonly IScheduler scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnqueueHandler"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="logger">Optional. The logger.</param>
        public EnqueueHandler(IScheduler scheduler, ILogger<EnqueueHandler>? logger = null)
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
        public async Task<EnqueueResponse> ProcessAsync(EnqueueMessage message, IMessagingContext context, CancellationToken token)
        {
            if (message.ScheduledJob == null || message.ScheduledJobId == null)
            {
                throw new ArgumentException(
                    $"Both {nameof(message.ScheduledJob)} and {nameof(message.ScheduledJobId)} parameters in the {nameof(EnqueueMessage)} are not set. One of them is required.",
                    nameof(message.ScheduledJob));
            }

            var result = await this.scheduler.EnqueueAsync(
                message.ScheduledJob ?? message.ScheduledJobId,
                ctx => ctx
                    .Impersonate(context)
                    .Activity(message.Target, message.Arguments, message.Options)
                    .Trigger(message.Trigger),
                token).PreserveThreadContext();

            result.ThrowIfHasErrors();

            return new EnqueueResponse
            {
                Result = result,
            };
        }
    }
}