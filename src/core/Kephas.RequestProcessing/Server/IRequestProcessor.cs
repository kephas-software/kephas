// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequestProcessor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Application service for processing requests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing.Server
{
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Application service for processing requests.
    /// </summary>
    /// <remarks>
    /// The request processor is defined as a shared service.
    /// </remarks>
    [SharedAppServiceContract]
    [ContractClass(typeof(RequestProcessorContractClass))]
    public interface IRequestProcessor
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
    [ContractClassFor(typeof(IRequestProcessor))]
    internal abstract class RequestProcessorContractClass : IRequestProcessor
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