// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetLogLevelHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the get log level handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Messaging;

    /// <summary>
    /// A get log level handler.
    /// </summary>
    public class GetLogLevelHandler : MessageHandlerBase<GetLogLevelMessage, GetLogLevelResponseMessage>
    {
        private readonly ILogManager logManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetLogLevelHandler"/> class.
        /// </summary>
        /// <param name="logManager">Manager for log.</param>
        public GetLogLevelHandler(ILogManager logManager)
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
        public override async Task<GetLogLevelResponseMessage> ProcessAsync(GetLogLevelMessage message, IMessagingContext context, CancellationToken token)
        {
            return new GetLogLevelResponseMessage
            {
                MinimumLevel = this.logManager.MinimumLevel,
                Message = $"The application's minimum log level is {this.logManager.MinimumLevel}.",
            };
        }
    }
}