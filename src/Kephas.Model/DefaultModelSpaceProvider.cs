// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultModelSpaceProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   The default implementation of a model space provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Model.Construction;
    using Kephas.Model.Construction.Internal;
    using Kephas.Model.Elements;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Services;
    using Kephas.Services.Transitions;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The default implementation of a model space provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultModelSpaceProvider : Loggable, IModelSpaceProvider
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
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="modelInfoProviders">The model information providers.</param>
        /// <param name="runtimeModelElementFactory">The runtime model element factory.</param>
        public DefaultModelSpaceProvider(
            IContextFactory contextFactory,
            ICollection<IModelInfoProvider> modelInfoProviders,
            IRuntimeModelElementFactory runtimeModelElementFactory)
            : base(contextFactory)
        {
            contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            runtimeModelElementFactory = runtimeModelElementFactory ?? throw new System.ArgumentNullException(nameof(runtimeModelElementFactory));
            modelInfoProviders = modelInfoProviders ?? throw new System.ArgumentNullException(nameof(modelInfoProviders));

            this.ContextFactory = contextFactory;
            this.ModelInfoProviders = modelInfoProviders;
            this.runtimeModelElementFactory = runtimeModelElementFactory;
        }

        /// <summary>
        /// Gets the context factory.
        /// </summary>
        /// <value>
        /// The context factory.
        /// </value>
        public IContextFactory ContextFactory { get; }

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
        public async Task InitializeAsync(IContext? context = null, CancellationToken cancellationToken = default)
        {
            this.initialization.Start();

            var constructionContext = this.CreateModelConstructionContext(context);
            var modelSpace = this.CreateModelSpace(constructionContext);
            constructionContext.ModelSpace = modelSpace;

            try
            {
                var elementInfosCollectorTask = Task.WhenAll(this.ModelInfoProviders.Select(p => p.GetElementInfosAsync(constructionContext, cancellationToken)));
                var elementInfos = (await elementInfosCollectorTask.PreserveThreadContext()).SelectMany(e => e).ToList();

                constructionContext.ElementInfos = elementInfos;
                var constructibleModelSpace = (IConstructibleElement)modelSpace;
                constructibleModelSpace.CompleteConstruction(constructionContext);
                if (constructibleModelSpace.ConstructionState.IsFaulted)
                {
                    throw constructibleModelSpace.ConstructionState.Exception!;
                }

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
        /// Creates the model construction context.
        /// </summary>
        /// <param name="parentContext">Context for the parent.</param>
        /// <returns>
        /// The new model construction context.
        /// </returns>
        protected virtual ModelConstructionContext CreateModelConstructionContext(IContext? parentContext)
        {
            var constructionContext = this.ContextFactory.CreateContext<ModelConstructionContext>(parentContext);
            constructionContext.RuntimeModelElementFactory = this.runtimeModelElementFactory;

            constructionContext.TryGetModelElementInfo =
                nativeElementInfo => this.ModelInfoProviders
                    .Select(p => p.TryGetModelElementInfo(nativeElementInfo, constructionContext))
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
            constructionContext = constructionContext ?? throw new System.ArgumentNullException(nameof(constructionContext));

            return new DefaultModelSpace(constructionContext);
        }
    }
}