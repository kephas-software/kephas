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
        /// <returns>The response promise.</returns>
        public Task<IResponse> ProcessAsync(IRequest request)
        {
            return Task.Factory.StartNew(() => this.Process(request));
        }

        /// <summary>
        /// Processes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        public IResponse Process(IRequest request)
        {
            var requestHandlerType = typeof(IRequestHandler<>).MakeGenericType(request.GetType());
            using (var requestHandler = (IRequestHandler)this.CompositionContainer.GetExport(requestHandlerType))
            {
                var response = requestHandler.Process(request);
                return response;
            }
        }
    }
}