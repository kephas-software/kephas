// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelTriggerHandler.cs" company="Kephas Software SRL">
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
    /// Handler for the <see cref="CancelTriggerMessage"/>.
    /// </summary>
    public class CancelTriggerHandler : MessageHandlerBase<CancelTriggerMessage, CancelTriggerResponseMessage>
    {
        private readonly IScheduler scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelTriggerHandler"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public CancelTriggerHandler(IScheduler scheduler, ILogManager? logManager = null)
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
        public override async Task<CancelTriggerResponseMessage> ProcessAsync(CancelTriggerMessage message, IMessagingContext context, CancellationToken token)
        {
            if (message.Trigger == null || message.TriggerId == null)
            {
                throw new ArgumentException(
                    $"Both {nameof(message.Trigger)} and {nameof(message.TriggerId)} parameters in the {nameof(CancelTriggerMessage)} are not set. One of them is required.",
                    nameof(message.Trigger));
            }

            var result = await this.scheduler.CancelTriggerAsync(
                message.Trigger ?? message.TriggerId,
                ctx => ctx.Impersonate(context),
                token).PreserveThreadContext();

            result.ThrowIfHasErrors();

            return new CancelTriggerResponseMessage
            {
                Result = result,
            };
        }
    }
}