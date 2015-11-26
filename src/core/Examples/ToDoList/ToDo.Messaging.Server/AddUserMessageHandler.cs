// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddUserMessageHandler.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   An add user message handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ToDo.Messaging.Server
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Messaging.Server;

    /// <summary>
    /// An add user message handler.
    /// </summary>
    public class AddUserMessageHandler : MessageHandlerBase<AddUserMessage, OperationResultMessage>
    {
        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public override Task<OperationResultMessage> ProcessAsync(AddUserMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}