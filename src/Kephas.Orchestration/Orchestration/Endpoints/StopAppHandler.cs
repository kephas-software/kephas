﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopAppHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the stop application handler class.
// </summary>
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
    using Kephas.Messaging.AttributedModel;
    using Kephas.Messaging.Distributed;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A stop application message handler.
    /// </summary>
    [MessageHandler(MessageTypeMatching.TypeOrHierarchy)]
    public class StopAppHandler : IMessageHandler<StopAppMessage, StopAppResponse>
    {
        private readonly IAppRuntime appRuntime;
        private readonly IEventHub eventHub;
        private readonly IMessageBroker messageBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="StopAppHandler"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="messageBroker">The message broker.</param>
        public StopAppHandler(IAppRuntime appRuntime, IEventHub eventHub, IMessageBroker messageBroker)
        {
            this.appRuntime = appRuntime;
            this.eventHub = eventHub;
            this.messageBroker = messageBroker;
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
        public async Task<StopAppResponse> ProcessAsync(StopAppMessage message, IMessagingContext context, CancellationToken token)
        {
            if (string.IsNullOrEmpty(message.AppId) && string.IsNullOrEmpty(message.AppInstanceId))
            {
                // TODO localization
                throw new InvalidOperationException($"Either the {nameof(message.AppId)} or the {nameof(message.AppInstanceId)} mut be set.");
            }

            var response = new StopAppResponse
            {
                ProcessId = Process.GetCurrentProcess().Id,
            };

            if ((message.AppInstanceId == this.appRuntime.GetAppInstanceId() && (message.AppId == this.appRuntime.GetAppId() || string.IsNullOrEmpty(message.AppId)))
                || (message.AppId == this.appRuntime.GetAppId() && string.IsNullOrEmpty(message.AppInstanceId)))
            {
                await this.eventHub.PublishAsync<InteractionContext>(
                    new ShutdownSignal("Handle stop app instance."),
                    ctx => ctx.Impersonate(context).OneWay().WithDelay(TimeSpan.FromSeconds(2)),
                    token).PreserveThreadContext();
                response.Message = $"App instance {this.appRuntime.GetAppId()}/{this.appRuntime.GetAppInstanceId()}: Shutdown request received. Good bye!";
            }
            else
            {
                await this.messageBroker.ProcessOneWayAsync(
                    message,
                    new Endpoint(appId: message.AppId, appInstanceId: message.AppInstanceId),
                    ctx => ctx.Impersonate(context),
                    cancellationToken: token).PreserveThreadContext();

                response.Message = $"App instance {this.appRuntime.GetAppId()}/{this.appRuntime.GetAppInstanceId()}: Shutdown request received, but it is addressed to another application instance(s). Delegated to {message.AppId}/{message.AppInstanceId}.";
            }

            return response;
        }
    }
}