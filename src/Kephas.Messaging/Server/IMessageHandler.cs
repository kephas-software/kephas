// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandler.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Application service for handling messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Server
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Application service for handling messages.
    /// </summary>
    [ContractClass(typeof(MessageHandlerContractClass))]
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
    [AppServiceContract]
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

    /// <summary>
    /// Contract class for <see cref="IMessageHandler"/>.
    /// </summary>
    [ContractClassFor(typeof(IMessageHandler))]
    internal abstract class MessageHandlerContractClass : IMessageHandler
    {
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
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
        public Task<IMessage> ProcessAsync(IMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            Requires.NotNull(message, nameof(message));
            Requires.NotNull(context, nameof(context));
            Contract.Ensures(Contract.Result<Task<IMessage>>() != null);

            return Contract.Result<Task<IMessage>>();
        }
    }
}