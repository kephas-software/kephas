// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncRequestProcessor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for asynchronous request processors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing
{
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Contract for asynchronous request processors.
    /// </summary>
    [ContractClass(typeof(AsyncRequestProcessorContractClass))]
    public interface IAsyncRequestProcessor
    {
        /// <summary>
        /// Processes the specified request asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        Task<IResponse> ProcessAsync(IRequest request, CancellationToken token);
    }

    /// <summary>
    /// Contract class for <see cref="IRequestProcessor"/>.
    /// </summary>
    [ContractClassFor(typeof(IAsyncRequestProcessor))]
    internal abstract class AsyncRequestProcessorContractClass : IAsyncRequestProcessor
    {
        /// <summary>
        /// Processes the specified request asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public Task<IResponse> ProcessAsync(IRequest request, CancellationToken token)
        {
            Contract.Requires(request != null);
            Contract.Ensures(Contract.Result<Task<IResponse>>() != null);

            return Contract.Result<Task<IResponse>>();
        }
    }
}