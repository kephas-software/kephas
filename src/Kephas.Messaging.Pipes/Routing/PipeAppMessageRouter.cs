// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipeAppMessageRouter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Pipes.Routing
{
    using Kephas.Application;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Services;

    /// <summary>
    /// A message router over named pipes.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    [MessageRouter(ReceiverMatch = ChannelType + ":.*", IsFallback = true)]
    public class PipeAppMessageRouter : InProcessAppMessageRouter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipeAppMessageRouter"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="messageProcessor">The message processor.</param>
        public PipeAppMessageRouter(IContextFactory contextFactory, IAppRuntime appRuntime, IMessageProcessor messageProcessor)
            : base(contextFactory, appRuntime, messageProcessor)
        {
        }
    }
}
