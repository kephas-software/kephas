// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFeatureManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IFeatureManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Shared service contract for managers of features within the application.
    /// </summary>
    /// <remarks>
    /// An application feature is a functional area of the application.
    /// It supports initialization and finalization.
    /// </remarks>
    [SharedAppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(FeatureInfoAttribute) })]
    public interface IFeatureManager : IAsyncInitializable, IAsyncFinalizable
    {
        /// <summary>
        /// Performs initialization tasks within the application feature boundary.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task InitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs finalization tasks within the application feature boundary.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task FinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = default);
    }
}