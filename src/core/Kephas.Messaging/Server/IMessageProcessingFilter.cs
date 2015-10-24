// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageProcessingFilter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Application service for message processing interception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Server
{
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Application service for message processing interception.
    /// </summary>
    [ContractClass(typeof(MessageProcessingFilterContractClass))]
    public interface IMessageProcessingFilter
    {
        /// <summary>
        /// Interception called before invoking the handler to process the message.
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task.</returns>
        Task BeforeProcessAsync(IMessageProcessingContext context, CancellationToken token);

        /// <summary>
        /// Interception called after invoking the handler to process the message.
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task.</returns>
        /// <remarks>
        /// The context will contain the response returned by the handler. 
        /// The interceptor may change the response or even replace it with another one.
        /// </remarks>
        Task AfterProcessAsync(IMessageProcessingContext context, CancellationToken token);
    }

    /// <summary>
    /// Application service for message processing interception.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    [AppServiceContract(AllowMultiple = true, 
        MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) }, 
        ContractType = typeof(IMessageProcessingFilter))]
    public interface IMessageProcessingFilter<TMessage> : IMessageProcessingFilter
        where TMessage : IMessage
    {
    }

    /// <summary>
    /// Contract class for <see cref="IMessageProcessingFilter"/>.
    /// </summary>
    [ContractClassFor(typeof(IMessageProcessingFilter))]
    internal abstract class MessageProcessingFilterContractClass : IMessageProcessingFilter
    {
        /// <summary>
        /// Interception called before invoking the handler to process the message.
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// A task.
        /// </returns>
        public Task BeforeProcessAsync(IMessageProcessingContext context, CancellationToken token)
        {
            Contract.Requires(context != null);
            Contract.Ensures(Contract.Result<Task>() != null);

            return Contract.Result<Task>();
        }

        /// <summary>
        /// Interception called after invoking the handler to process the message.
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// A task.
        /// </returns>
        /// <remarks>
        /// The context will contain the response returned by the handler.
        /// The interceptor may change the response or even replace it with another one.
        /// </remarks>
        public Task AfterProcessAsync(IMessageProcessingContext context, CancellationToken token)
        {
            Contract.Requires(context != null);
            Contract.Ensures(Contract.Result<Task>() != null);

            return Contract.Result<Task>();
        }
    }
}