// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultModelSpaceProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default implementation of a model space provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Services;
    using Kephas.Services.Transitioning;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The default implementation of a model space provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultModelSpaceProvider : IModelSpaceProvider
    {
        /// <summary>
        /// Monitors the initialization state.
        /// </summary>
        private readonly InitializationMonitor<IModelSpaceProvider, DefaultModelSpaceProvider> initialization = new InitializationMonitor<IModelSpaceProvider, DefaultModelSpaceProvider>();

        /// <summary>
        /// The model space.
        /// </summary>
        private IModelSpace modelSpace;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultModelSpaceProvider"/> class.
        /// </summary>
        /// <param name="modelInfoProviders">
        /// The model information providers.
        /// </param>
        public DefaultModelSpaceProvider(ICollection<IModelInfoProvider> modelInfoProviders)
        {
            this.ModelInfoProviders = modelInfoProviders;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<DefaultModelSpaceProvider> Logger { get; set; }

        /// <summary>
        /// Gets the model information providers.
        /// </summary>
        /// <value>
        /// The model information providers.
        /// </value>
        public ICollection<IModelInfoProvider> ModelInfoProviders { get; private set; }

        /// <summary>
        /// Gets the model space.
        /// </summary>
        /// <returns>
        /// The model space.
        /// </returns>
        public IModelSpace GetModelSpace()
        {
            this.initialization.AssertIsCompletedSuccessfully();

            return this.modelSpace;
        }

        /// <summary>
        /// Initializes the service asynchronously by loading the model space.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        public async Task InitializeAsync(IContext context = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.initialization.Start();

            var modelSpace = new DefaultModelSpace(constructionContext);
            var constructionContext = new ModelConstructionContext { ModelSpace = modelSpace };

            try
            {
                var elementInfosCollectorTask = Task.WhenAll(this.ModelInfoProviders.Select(p => p.GetElementInfosAsync(constructionContext, cancellationToken)));
                var elementInfos = (await elementInfosCollectorTask.WithServerThreadingContext()).SelectMany(e => e);

                // collect the model dimensions and dimension elements, completing their construction
                // build the model projections
                // complete the construction of the other model elements, assigning them to the right projection
                // build the aggregated projections
                // aggregate the model elements, adding them to the right aggregated projection

                throw new NotImplementedException();

                this.modelSpace = modelSpace;

                this.initialization.Complete();
            }
            catch (Exception exception)
            {
                this.initialization.Fault(exception);
                throw;
            }
        }
    }
}