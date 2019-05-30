// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InProcessMessageBroker.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null distributed message broker class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Messaging.Distributed.Composition;
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
        /// <param name="messageBuilderFactories">The message builder factories.</param>
        /// <param name="messageProcessor">The message processor.</param>
        public InProcessMessageBroker(ICollection<IExportFactory<IBrokeredMessageBuilder, BrokeredMessageBuilderMetadata>> messageBuilderFactories, IMessageProcessor messageProcessor)
            : base(messageBuilderFactories)
        {
            Requires.NotNull(messageProcessor, nameof(messageProcessor));

            this.messageProcessor = messageProcessor;
        }

        /// <summary>
        /// Sends the brokered message asynchronously over the physical medium.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The sending context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result that yields an IMessage.
        /// </returns>
        protected override Task SendAsync(
            IBrokeredMessage brokeredMessage,
            IContext context,
            CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(
                async () =>
                {
                    try
                    {
                        var messagingContext = new MessageProcessingContext(this.messageProcessor, brokeredMessage)
                                          {
                                              Identity = context?.Identity
                                          };
                        return await this.messageProcessor.ProcessAsync(brokeredMessage, messagingContext, cancellationToken)
                                   .PreserveThreadContext();
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Warn(ex, Strings.InProcessMessageBroker_MessageProcessor_Async_Exception);
                        return null;
                    }
                },
                cancellationToken);
        }
    }
}