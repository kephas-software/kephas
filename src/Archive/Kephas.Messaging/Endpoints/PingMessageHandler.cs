// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingMessageHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Message handler for the <see cref="PingMessage" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Endpoints
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Messaging.Messages;

    /// <summary>
    /// Message handler for the <see cref="PingMessage"/>.
    /// </summary>
    public class PingMessageHandler : IMessageHandler<PingMessage, PingBack>
    {
        private readonly IAppRuntime appRuntime;

        /// <summary>
        /// Initializes a new instance of the <see cref="PingMessageHandler"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        public PingMessageHandler(IAppRuntime appRuntime)
        {
            this.appRuntime = appRuntime;
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
        public Task<PingBack> ProcessAsync(PingMessage message, IMessagingContext context, CancellationToken token)
        {
            return Task.FromResult(new PingBack
                                       {
                                           Message = $"Hello from app {this.appRuntime.GetAppId()}, instance {this.appRuntime.GetAppInstanceId()}.",
                                           ServerTime = DateTimeOffset.Now,
                                       });
        }
    }
}