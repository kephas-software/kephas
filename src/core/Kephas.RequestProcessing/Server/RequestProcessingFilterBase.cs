// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestProcessingFilterBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base implementation of a request processing filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing.Server
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base implementation of a request processing filter.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    public abstract class RequestProcessingFilterBase<TRequest> : IRequestProcessingFilter<TRequest>
        where TRequest : IRequest
    {
        /// <summary>
        /// Interception called before invoking the handler to process the request.
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task.</returns>
        Task IRequestProcessingFilter.BeforeProcessAsync(IProcessingContext context, CancellationToken token)
        {
            var request = (TRequest)context.Request;
            return this.BeforeProcessAsync(request, context, token);
        }

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
        Task IRequestProcessingFilter.AfterProcessAsync(IProcessingContext context, CancellationToken token)
        {
            var request = (TRequest)context.Request;
            return this.AfterProcessAsync(request, context, token);
        }

        /// <summary>
        /// Interception called before invoking the handler to process the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// A task.
        /// </returns>
        public virtual Task BeforeProcessAsync(TRequest request, IProcessingContext context, CancellationToken token)
        {
            return TaskHelper.EmptyTask<bool>();
        }

        /// <summary>
        /// Interception called after invoking the handler to process the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// A task.
        /// </returns>
        /// <remarks>
        /// The context will contain the response returned by the handler.
        /// The interceptor may change the response or even replace it with another one.
        /// </remarks>
        public Task AfterProcessAsync(TRequest request, IProcessingContext context, CancellationToken token)
        {
            return TaskHelper.EmptyTask<bool>();
        }
    }
}