// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetLogLevelHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the set log level handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;

    /// <summary>
    /// A set log level handler.
    /// </summary>
    public class SetLogLevelHandler : MessageHandlerBase<SetLogLevelMessage, ResponseMessage>
    {
        private readonly ILogManager logManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetLogLevelHandler"/> class.
        /// </summary>
        /// <param name="logManager">Manager for log.</param>
        public SetLogLevelHandler(ILogManager logManager)
            : base(logManager)
        {
            this.logManager = logManager;
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
        public override async Task<ResponseMessage> ProcessAsync(SetLogLevelMessage message, IMessagingContext context, CancellationToken token)
        {
            this.logManager.MinimumLevel = message.MinimumLevel;
            return new ResponseMessage
            {
                Message = $"The application's minimum log level was set to {this.logManager.MinimumLevel}.",
            };
        }
    }
}