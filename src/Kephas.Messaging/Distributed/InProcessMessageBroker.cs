// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InProcessMessageBroker.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null distributed message broker class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Messaging.Resources;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A distributed message broker sending the messages to the message processor.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class InProcessMessageBroker : MessageBrokerBase
    {
        /// <summary>
        /// The message processor.
        /// </summary>
        private readonly IMessageProcessor messageProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcessMessageBroker"/> class.
        /// </summary>
        /// <param name="appManifest">The application manifest.</param>
        /// <param name="messageProcessor">The message processor.</param>
        public InProcessMessageBroker(IAppManifest appManifest, IMessageProcessor messageProcessor)
            : base(appManifest)
        {
            Requires.NotNull(messageProcessor, nameof(messageProcessor));

            this.messageProcessor = messageProcessor;
        }

        /// <summary>
        /// Sends the brokered message asynchronously over the physical medium.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result that yields an IMessage.
        /// </returns>
        protected override Task SendAsync(
            IBrokeredMessage brokeredMessage,
            CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        return await this.messageProcessor.ProcessAsync(brokeredMessage, null, cancellationToken)
                                   .PreserveThreadContext();
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Warn(ex, Strings.InProcessMessageBroker_MessageProcessor_Async_Exception);
                        return null;
                    }
                }, cancellationToken);
        }
    }
}