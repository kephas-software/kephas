// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestartHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Endpoints
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Interaction;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Messages;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A restart message handler.
    /// </summary>
    public class RestartHandler : MessageHandlerBase<RestartMessage, ResponseMessage>
    {
        private readonly IAppRuntime appRuntime;
        private readonly IEventHub eventHub;
        private readonly IMessageBroker messageBroker;
        private readonly Lazy<IOrchestrationManager> lazyOrchestrationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestartHandler"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="lazyOrchestrationManager">The lazy orchestration manager.</param>
        public RestartHandler(
            IAppRuntime appRuntime,
            IEventHub eventHub,
            IMessageBroker messageBroker,
            Lazy<IOrchestrationManager> lazyOrchestrationManager)
        {
            this.appRuntime = appRuntime;
            this.eventHub = eventHub;
            this.messageBroker = messageBroker;
            this.lazyOrchestrationManager = lazyOrchestrationManager;
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
        public override async Task<ResponseMessage?> ProcessAsync(RestartMessage message, IMessagingContext context, CancellationToken token)
        {
            var response = new ResponseMessage();

            if (this.appRuntime.IsRoot())
            {
                await this.eventHub.PublishAsync(new RestartSignal("Handle restart."), context, token).PreserveThreadContext();
                response.Message = $"App instance {this.appRuntime.GetAppId()}/{this.appRuntime.GetAppInstanceId()}: Application restarted.";
            }
            else
            {
                // redirect only once.
                await this.messageBroker.ProcessOneWayAsync(
                    message,
                    new Endpoint(appInstanceId: this.lazyOrchestrationManager.Value.GetRootAppInstanceId()),
                    ctx => ctx.Impersonate(context),
                    cancellationToken: token).PreserveThreadContext();

                response.Message = $"App instance {this.appRuntime.GetAppId()}/{this.appRuntime.GetAppInstanceId()}: Restart request received, but it is addressed to another application instance(s). Delegated to root.";
            }

            return response;
        }
    }
}