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
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Model.Construction;
    using Kephas.Model.Construction.Internal;
    using Kephas.Model.Elements;
    using Kephas.Model.Runtime.Construction;
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
        /// The runtime model element factory.
        /// </summary>
        private readonly IRuntimeModelElementFactory runtimeModelElementFactory;

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
        /// <param name="modelInfoProviders">The model information providers.</param>
        /// <param name="runtimeModelElementFactory">The runtime model element factory.</param>
        public DefaultModelSpaceProvider(ICollection<IModelInfoProvider> modelInfoProviders, IRuntimeModelElementFactory runtimeModelElementFactory)
        {
            Contract.Requires(runtimeModelElementFactory != null);
            Contract.Requires(modelInfoProviders != null);

            this.ModelInfoProviders = modelInfoProviders;
            this.runtimeModelElementFactory = runtimeModelElementFactory;
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
        public ICollection<IModelInfoProvider> ModelInfoProviders { get; }

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

            var constructionContext = new ModelConstructionContext { RuntimeModelElementFactory = this.runtimeModelElementFactory };
            var modelSpace = this.CreateModelSpace(constructionContext);
            constructionContext.ModelSpace = modelSpace;

            try
            {
                var elementInfosCollectorTask = Task.WhenAll(this.ModelInfoProviders.Select(p => p.GetElementInfosAsync(constructionContext, cancellationToken)));
                var elementInfos = (await elementInfosCollectorTask.PreserveThreadContext()).SelectMany(e => e).ToList();

                constructionContext[nameof(IModelConstructionContext.ElementInfos)] = elementInfos;
                ((IWritableNamedElement)modelSpace).CompleteConstruction(constructionContext);

                this.modelSpace = modelSpace;

                this.initialization.Complete();
            }
            catch (Exception exception)
            {
                this.initialization.Fault(exception);
                throw;
            }
        }

        /// <summary>
        /// Creates the model space.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <returns>
        /// The new model space.
        /// </returns>
        protected virtual IModelSpace CreateModelSpace(IModelConstructionContext constructionContext)
        {
            Contract.Requires(constructionContext != null);

            return new DefaultModelSpace(constructionContext);
        }
    }
}