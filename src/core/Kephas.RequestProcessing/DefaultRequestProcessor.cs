// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRequestProcessor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provides the default implementation of the <see cref="IRequestProcessor" /> application service contract.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing
{
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Services;

    /// <summary>
    /// Provides the default implementation of the <see cref="IRequestProcessor"/> application service contract.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultRequestProcessor : IRequestProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRequestProcessor"/> class.
        /// </summary>
        /// <param name="compositionContainer">The composition container.</param>
        public DefaultRequestProcessor(ICompositionContainer compositionContainer)
        {
            Contract.Requires(compositionContainer != null);

            this.CompositionContainer = compositionContainer;
        }

        /// <summary>
        /// Gets the composition container.
        /// </summary>
        /// <value>
        /// The composition container.
        /// </value>
        public ICompositionContainer CompositionContainer { get; private set; }

        /// <summary>
        /// Processes the specified request asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public async Task<IResponse> ProcessAsync(IRequest request, CancellationToken token)
        {
            using (var requestHandler = this.CreateRequestHandler(request))
            {
                var response = await requestHandler.ProcessAsync(request, token);
                return response;
            }
        }

        /// <summary>
        /// Creates the request handler.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The newly created request handler.</returns>
        protected virtual IRequestHandler CreateRequestHandler(IRequest request)
        {
            var requestHandlerType = typeof(IRequestHandler<>).MakeGenericType(request.GetType());
            var requestHandler = (IRequestHandler)this.CompositionContainer.GetExport(requestHandlerType);
            return requestHandler;
        }
    }
}