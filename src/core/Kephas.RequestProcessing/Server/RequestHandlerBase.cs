// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestHandlerBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provides a base implementation of a request handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing.Server
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.RequestProcessing.Resources;

    /// <summary>
    /// Provides a base implementation of a request handler.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    public abstract class RequestHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest>
        where TRequest : class, IRequest
        where TResponse : class, IResponse
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
        public abstract Task<TResponse> ProcessAsync(TRequest request, IProcessingContext context, CancellationToken token);

        /// <summary>
        /// Processes the provided request asynchronously and returns a response promise.
        /// </summary>
        /// <param name="request">The request to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        async Task<IResponse> IRequestHandler<TRequest>.ProcessAsync(TRequest request, IProcessingContext context, CancellationToken token)
        {
            var response = await this.ProcessAsync(request, context, token);
            return response;
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
        async Task<IResponse> IRequestHandler.ProcessAsync(IRequest request, IProcessingContext context, CancellationToken token)
        {
            var typedRequest = request as TRequest;
            if (typedRequest == null)
            {
                throw new ArgumentException(string.Format(Strings.RequestHandlerBadRequestType, typeof(TRequest)), "request");
            }

            var response = await this.ProcessAsync(typedRequest, context, token);
            return response;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}