// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequestHandler.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Application service for handling requests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing.Server
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Application service for handling requests.
    /// </summary>
    [ContractClass(typeof(RequestHandlerContractClass))]
    public interface IRequestHandler : IDisposable
    {
        /// <summary>
        /// Processes the provided request asynchronously and returns a response promise.
        /// </summary>
        /// <param name="request">The request to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        Task<IResponse> ProcessAsync(IRequest request, IProcessingContext context, CancellationToken token);
    }

    /// <summary>
    /// Application service for handling requests.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    [AppServiceContract]
    public interface IRequestHandler<in TRequest> : IRequestHandler
        where TRequest : IRequest
    {
        /// <summary>
        /// Processes the provided request asynchronously and returns a response promise.
        /// </summary>
        /// <param name="request">The request to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        Task<IResponse> ProcessAsync(TRequest request, IProcessingContext context, CancellationToken token);
    }

    /// <summary>
    /// Contract class for <see cref="IRequestHandler"/>.
    /// </summary>
    [ContractClassFor(typeof(IRequestHandler))]
    internal abstract class RequestHandlerContractClass : IRequestHandler
    {
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Processes the provided request asynchronously and returns a response promise.
        /// </summary>
        /// <param name="request">The request to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public Task<IResponse> ProcessAsync(IRequest request, IProcessingContext context, CancellationToken token)
        {
            Contract.Requires(request != null);
            Contract.Requires(context != null);
            Contract.Ensures(Contract.Result<Task<IResponse>>() != null);

            return Contract.Result<Task<IResponse>>();
        }
    }
}