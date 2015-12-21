// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageProcessor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Application service for processing messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Server
{
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Application service for processing messages.
    /// </summary>
    /// <remarks>
    /// The message processor is defined as a shared service.
    /// </remarks>
    [SharedAppServiceContract]
    [ContractClass(typeof(MessageProcessorContractClass))]
    public interface IMessageProcessor
    {
        /// <summary>
        /// Processes the specified message asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">Context for the message processing.</param>
        /// <param name="token">  The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        Task<IMessage> ProcessAsync(IMessage message, IMessageProcessingContext context = null, CancellationToken token = default(CancellationToken));
    }

    /// <summary>
    /// Contract class for <see cref="IMessageProcessor"/>.
    /// </summary>
    [ContractClassFor(typeof(IMessageProcessor))]
    internal abstract class MessageProcessorContractClass : IMessageProcessor
    {
        /// <summary>
        /// Processes the specified message asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">Context for the message processing.</param>
        /// <param name="token">  The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public Task<IMessage> ProcessAsync(IMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            Contract.Requires(message != null);
            Contract.Ensures(Contract.Result<Task<IMessage>>() != null);

            return Contract.Result<Task<IMessage>>();
        }
    }
}