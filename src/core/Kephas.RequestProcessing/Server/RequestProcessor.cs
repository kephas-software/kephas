// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestProcessor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provides the default implementation of the <see cref="IRequestProcessor" /> application service contract.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing.Server
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Services;

    /// <summary>
    /// Provides the default implementation of the <see cref="IRequestProcessor"/> application service contract.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class RequestProcessor : IRequestProcessor
    {
        /// <summary>
        /// The filter factories.
        /// </summary>
        private readonly IList<IExportFactory<IRequestProcessingFilter, RequestProcessingFilterMetadata>> filterFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestProcessor" /> class.
        /// </summary>
        /// <param name="compositionContainer">The composition container.</param>
        /// <param name="filterFactories">The filter factories.</param>
        public RequestProcessor(ICompositionContainer compositionContainer, IList<IExportFactory<IRequestProcessingFilter, RequestProcessingFilterMetadata>> filterFactories)
        {
            Contract.Requires(compositionContainer != null);
            Contract.Requires(filterFactories != null);

            this.CompositionContainer = compositionContainer;
            this.filterFactories = filterFactories;
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
                var context = this.CreateProcessingContext(request, requestHandler);
                var filters = this.GetOrderedFilters(context);

                try
                {
                    foreach (var filter in filters)
                    {
                        await filter.BeforeProcessAsync(context, token);
                    }

                    var response = await requestHandler.ProcessAsync(request, context, token);
                    context.Response = response;
                }
                catch (Exception ex)
                {
                    context.Exception = ex;
                }

                foreach (var filter in filters.Reverse())
                {
                    await filter.AfterProcessAsync(context, token);
                }

                if (context.Exception != null)
                {
                    throw context.Exception;
                }

                return context.Response;
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

        /// <summary>
        /// Creates the processing context.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>The processing context.</returns>
        protected virtual IProcessingContext CreateProcessingContext(IRequest request, IRequestHandler handler)
        {
            Contract.Ensures(Contract.Result<IProcessingContext>() != null);

            return new ProcessingContext(request, handler);
        }

        /// <summary>
        /// Gets the ordered filters to be applied.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>An ordered list of filters which can be applied to the provided context.</returns>
        protected virtual IList<IRequestProcessingFilter> GetOrderedFilters(IProcessingContext context)
        {
            Contract.Ensures(Contract.Result<IList<IRequestProcessingFilter>>() != null);

            var requestTypeInfo = context.Request.GetType().GetTypeInfo();
            var behaviors = (from b in this.filterFactories
                             where b.Metadata.RequestType.GetTypeInfo().IsAssignableFrom(requestTypeInfo)
                             orderby b.Metadata.ProcessingPriority
                             select b.CreateExport().Value).ToList();

            // TODO optimize to cache the ordered filters/request type.
            return behaviors;
        }
    }
}