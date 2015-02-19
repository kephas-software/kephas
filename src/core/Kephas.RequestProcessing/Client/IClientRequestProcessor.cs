// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientRequestProcessor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Application service for clients of request processors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing.Client
{
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Application service for clients of request processors.
    /// </summary>
    /// <remarks>
    /// The client request processor is defined as a shared service.
    /// </remarks>
    [SharedAppServiceContract]
    [ContractClass(typeof(ClientRequestProcessorContractClass))]
    public interface IClientRequestProcessor
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
    /// Contract class for <see cref="IClientRequestProcessor"/>.
    /// </summary>
    [ContractClassFor(typeof(IClientRequestProcessor))]
    internal abstract class ClientRequestProcessorContractClass : IClientRequestProcessor
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