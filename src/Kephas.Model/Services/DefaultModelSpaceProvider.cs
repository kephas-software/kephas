// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultModelSpaceProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
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
    public class DefaultModelSpaceProvider : IModelSpaceProvider, ICompositionContextAware
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
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="modelInfoProviders">The model information providers.</param>
        /// <param name="runtimeModelElementFactory">The runtime model element factory.</param>
        public DefaultModelSpaceProvider(
            ICompositionContext compositionContext,
            ICollection<IModelInfoProvider> modelInfoProviders,
            IRuntimeModelElementFactory runtimeModelElementFactory)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));
            Requires.NotNull(runtimeModelElementFactory, nameof(runtimeModelElementFactory));
            Requires.NotNull(modelInfoProviders, nameof(modelInfoProviders));

            this.CompositionContext = compositionContext;
            this.ModelInfoProviders = modelInfoProviders;
            this.runtimeModelElementFactory = runtimeModelElementFactory;
        }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public ICompositionContext CompositionContext { get; }

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
        public async Task InitializeAsync(IContext context = null, CancellationToken cancellationToken = default)
        {
            this.initialization.Start();

            var constructionContext = this.CreateModelConstructionContext();
            var constructedModelSpace = this.CreateModelSpace(constructionContext);
            constructionContext.ModelSpace = constructedModelSpace;

            try
            {
                var elementInfosCollectorTask = Task.WhenAll(this.ModelInfoProviders.Select(p => p.GetElementInfosAsync(constructionContext, cancellationToken)));
                var elementInfos = (await elementInfosCollectorTask.PreserveThreadContext()).SelectMany(e => e).ToList();

                constructionContext.ElementInfos = elementInfos;
                var writableModelSpace = (IConstructableElement)constructedModelSpace;
                writableModelSpace.CompleteConstruction(constructionContext);
                if (writableModelSpace.ConstructionState.IsFaulted)
                {
                    throw writableModelSpace.ConstructionState.Exception;
                }

                this.modelSpace = constructedModelSpace;

                this.initialization.Complete();
            }
            catch (Exception exception)
            {
                this.initialization.Fault(exception);
                throw;
            }
        }

        /// <summary>
        /// Creates the model construction context.
        /// </summary>
        /// <returns>
        /// The new model construction context.
        /// </returns>
        protected virtual ModelConstructionContext CreateModelConstructionContext()
        {
            var constructionContext = new ModelConstructionContext(this.CompositionContext)
                {
                    RuntimeModelElementFactory = this.runtimeModelElementFactory,
                };

            constructionContext.TryGetElementInfo = 
                nativeElementInfo => this.ModelInfoProviders
                    .Select(p => p.TryGetElementInfo(nativeElementInfo, constructionContext))
                    .FirstOrDefault(p => p != null);

            return constructionContext;
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
            Requires.NotNull(constructionContext, nameof(constructionContext));

            return new DefaultModelSpace(constructionContext);
        }
    }
}