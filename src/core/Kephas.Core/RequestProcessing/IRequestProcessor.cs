// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequestProcessor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Application service for processing requests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing
{
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Application service for processing requests.
    /// </summary>
    /// <remarks>
    /// The request processor is defined as a shared service.
    /// </remarks>
    [ContractClass(typeof(RequestProcessorContractClass))]
    [AppServiceContract]
    public interface IRequestProcessor : IAsyncRequestProcessor
    {
        /// <summary>
        /// Processes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        IResponse Process(IRequest request);
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
        /// <returns>The response promise.</returns>
        public Task<IResponse> ProcessAsync(IRequest request)
        {
            throw new System.NotSupportedException();
        }

        /// <summary>
        /// Processes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        public IResponse Process(IRequest request)
        {
            Contract.Requires(request != null);
            Contract.Ensures(Contract.Result<IResponse>() != null);

            throw new System.NotSupportedException();
        }
    }
}