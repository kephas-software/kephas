// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetLiveAppsHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Endpoints
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Messaging;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Handler for the <see cref="GetLiveAppsMessage"/>.
    /// </summary>
    public class GetLiveAppsHandler : IMessageHandler<GetLiveAppsMessage, GetLiveAppsResponse>
    {
        private readonly IOrchestrationManager orchestrationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetLiveAppsHandler"/> class.
        /// </summary>
        /// <param name="orchestrationManager">The orchestration manager.</param>
        public GetLiveAppsHandler(IOrchestrationManager orchestrationManager)
        {
            this.orchestrationManager = orchestrationManager;
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
        public async Task<GetLiveAppsResponse> ProcessAsync(GetLiveAppsMessage message, IMessagingContext context, CancellationToken token)
        {
            var liveApps = await this.orchestrationManager.GetLiveAppsAsync(cancellationToken: token)
                .PreserveThreadContext();
            return new GetLiveAppsResponse
            {
                Apps = liveApps.ToArray(),
            };
        }
    }
}