// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppMessageHandlerBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the base application message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Endpoints
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging;

    /// <summary>
    /// A base application message handler.
    /// </summary>
    /// <typeparam name="TAppMessage">Type of the application message.</typeparam>
    public class AppMessageHandlerBase<TAppMessage> : MessageHandlerBase<TAppMessage, IMessage>
        where TAppMessage : class, IAppMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppMessageHandlerBase{TAppMessage}"/>
        /// class.
        /// </summary>
        /// <param name="orchestrationManager">Manager for orchestration.</param>
        public AppMessageHandlerBase(IOrchestrationManager orchestrationManager)
        {
            Requires.NotNull(orchestrationManager, nameof(orchestrationManager));

            this.OrchestrationManager = orchestrationManager;
        }

        /// <summary>
        /// Gets the manager for orchestration.
        /// </summary>
        /// <value>
        /// The orchestration manager.
        /// </value>
        public IOrchestrationManager OrchestrationManager { get; }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public override async Task<IMessage> ProcessAsync(TAppMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            // TODO

            throw new System.NotImplementedException();
        }
    }
}