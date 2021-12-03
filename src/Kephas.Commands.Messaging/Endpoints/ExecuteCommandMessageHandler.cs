// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecuteCommandMessageHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Endpoints
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Message handler for <see cref="ExecuteCommandMessage"/>.
    /// </summary>
    public class ExecuteCommandMessageHandler : MessageHandlerBase<ExecuteCommandMessage, ExecuteCommandResponseMessage>
    {
        private readonly Lazy<ICommandProcessor> lazyCommandProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteCommandMessageHandler"/> class.
        /// </summary>
        /// <param name="lazyCommandProcessor">The lazy command processor.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public ExecuteCommandMessageHandler(Lazy<ICommandProcessor> lazyCommandProcessor, ILogManager? logManager = null)
            : base(logManager)
        {
            this.lazyCommandProcessor = lazyCommandProcessor;
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
        public override async Task<ExecuteCommandResponseMessage> ProcessAsync(
            ExecuteCommandMessage message,
            IMessagingContext context,
            CancellationToken token)
        {
            if (string.IsNullOrEmpty(message.Command)) throw new ArgumentException("The command name must not be null or empty.", nameof(message.Command));

            var result = await this.lazyCommandProcessor.Value
                .ProcessAsync(message.Command!, message.Args, context, token).PreserveThreadContext();

            return new ExecuteCommandResponseMessage
            {
                ReturnValue = result,
            };
        }
    }
}