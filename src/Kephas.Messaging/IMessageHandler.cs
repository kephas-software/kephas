// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandler.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Application service for handling messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Application service for handling messages.
    /// </summary>
    public interface IMessageHandler : IDisposable
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
        Task<IMessage> ProcessAsync(IMessage message, IMessageProcessingContext context, CancellationToken token);
    }

    /// <summary>
    /// Application service for handling requests.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    [AppServiceContract(ContractType = typeof(IMessageHandler), MetadataAttributes = new[] { typeof(MessageNameAttribute) })]
    public interface IMessageHandler<in TMessage> : IMessageHandler
        where TMessage : IMessage
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
        Task<IMessage> ProcessAsync(TMessage message, IMessageProcessingContext context, CancellationToken token);
    }
}