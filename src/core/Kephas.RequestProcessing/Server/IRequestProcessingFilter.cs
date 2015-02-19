// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequestProcessingFilter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Application service for request processing interception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing.Server
{
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Application service for request processing interception.
    /// </summary>
    [ContractClass(typeof(RequestProcessingFilterContractClass))]
    public interface IRequestProcessingFilter
    {
        /// <summary>
        /// Interception called before invoking the handler to process the request.
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task.</returns>
        Task BeforeProcessAsync(IProcessingContext context, CancellationToken token);

        /// <summary>
        /// Interception called after invoking the handler to process the request.
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task.</returns>
        /// <remarks>
        /// The context will contain the response returned by the handler. 
        /// The interceptor may change the response or even replace it with another one.
        /// </remarks>
        Task AfterProcessAsync(IProcessingContext context, CancellationToken token);
    }

    /// <summary>
    /// Application service for request processing interception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    [AppServiceContract(AllowMultiple = true, 
        MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) }, 
        ContractType = typeof(IRequestProcessingFilter))]
    public interface IRequestProcessingFilter<TRequest> : IRequestProcessingFilter
        where TRequest : IRequest
    {
    }

    /// <summary>
    /// Contract class for <see cref="IRequestProcessingFilter"/>.
    /// </summary>
    [ContractClassFor(typeof(IRequestProcessingFilter))]
    internal abstract class RequestProcessingFilterContractClass : IRequestProcessingFilter
    {
        /// <summary>
        /// Interception called before invoking the handler to process the request.
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// A task.
        /// </returns>
        public Task BeforeProcessAsync(IProcessingContext context, CancellationToken token)
        {
            Contract.Requires(context != null);
            Contract.Ensures(Contract.Result<Task>() != null);

            return Contract.Result<Task>();
        }

        /// <summary>
        /// Interception called after invoking the handler to process the request.
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
        public Task AfterProcessAsync(IProcessingContext context, CancellationToken token)
        {
            Contract.Requires(context != null);
            Contract.Ensures(Contract.Result<Task>() != null);

            return Contract.Result<Task>();
        }
    }
}