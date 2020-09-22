// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelRunningJobHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Kephas.Operations;
using Kephas.Services;
using Kephas.Threading.Tasks;

namespace Kephas.Scheduling.Endpoints
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Messaging;

    /// <summary>
    /// Message handler for <see cref="CancelRunningJobMessage"/>.
    /// </summary>
    public class CancelRunningJobHandler : MessageHandlerBase<CancelRunningJobMessage, CancelRunningJobResponseMessage>
    {
        private readonly IScheduler scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelRunningJobHandler"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public CancelRunningJobHandler(IScheduler scheduler, ILogManager? logManager = null)
            : base(logManager)
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
        public override async Task<CancelRunningJobResponseMessage> ProcessAsync(CancelRunningJobMessage message, IMessagingContext context, CancellationToken token)
        {
            if (message.RunningJob == null || message.RunningJobId == null)
            {
                throw new ArgumentException(
                    $"Both {nameof(message.RunningJob)} and {nameof(message.RunningJobId)} parameters in the {nameof(CancelRunningJobMessage)} are not set. One of them is required.",
                    nameof(message.RunningJob));
            }

            var result = await this.scheduler.CancelRunningJobAsync(
                message.RunningJob ?? message.RunningJobId,
                ctx => ctx.Impersonate(context),
                token).PreserveThreadContext();

            result.ThrowIfHasErrors();

            return new CancelRunningJobResponseMessage
            {
                Result = result,
            };
        }
    }
}