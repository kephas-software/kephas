// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnableScheduledJobHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Endpoints
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.ExceptionHandling;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Operations;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Handler for enabling a scheduled job.
    /// </summary>
    public class EnableScheduledJobHandler : IMessageHandler<EnableScheduledJobMessage, Response>
    {
        private readonly IScheduler scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableScheduledJobHandler"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        public EnableScheduledJobHandler(IScheduler scheduler)
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
        public async Task<Response> ProcessAsync(EnableScheduledJobMessage message, IMessagingContext context, CancellationToken token)
        {
            if (message.Job == null) throw new ArgumentNullException(nameof(message.Job));

            var result = await this.scheduler.EnableScheduledJobAsync(message.Job!, cancellationToken: token)
                .PreserveThreadContext();

            return new Response
            {
                Severity = result.HasErrors()
                    ? SeverityLevel.Error
                    : result.HasWarnings()
                        ? SeverityLevel.Warning
                        : SeverityLevel.Info,
                Message = result.HasErrors()
                    ? string.Join(Environment.NewLine, result.Errors().Select(ex => ex.Message))
                    : result.HasWarnings()
                        ? string.Join(Environment.NewLine, result.Warnings().Select(ex => ex.Message))
                        : string.Join(Environment.NewLine, result.Infos().Select(m => m.Message)),
            };
        }
    }
}