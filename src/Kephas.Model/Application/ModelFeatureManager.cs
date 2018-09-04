// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelFeatureManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Application initializer for the model space.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Feature manager for the model.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    [FeatureInfo(FeatureName, isRequired: true)]
    public class ModelFeatureManager : FeatureManagerBase
    {
        /// <summary>
        /// Name of the feature.
        /// </summary>
        public const string FeatureName = "Model";

        /// <summary>
        /// The model space provider.
        /// </summary>
        private readonly IModelSpaceProvider modelSpaceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelFeatureManager"/> class.
        /// </summary>
        /// <param name="modelSpaceProvider">The model space provider.</param>
        public ModelFeatureManager(IModelSpaceProvider modelSpaceProvider)
        {
            Requires.NotNull(modelSpaceProvider, nameof(modelSpaceProvider));

            this.modelSpaceProvider = modelSpaceProvider;
        }

        /// <summary>
        /// Initializes the model infrastructure asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            return this.modelSpaceProvider.InitializeAsync(appContext, cancellationToken);
        }
    }
}