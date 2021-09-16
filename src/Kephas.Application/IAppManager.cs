// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Singleton service contract for the application manager.
    /// </summary>
    /// <remarks>
    /// The application manager is a service whose concern is to initialize and finalize the application.
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// var appManager = compositionContext.Resolve&lt;IAppManager&gt;();
    /// var appContext = new AppContext();
    /// await appManager.InitializeAppManagerAsync(appContext);
    /// ...
    /// await appManager.FinalizeAppAsync(appContext);
    /// </code>
    /// </example>
    [SingletonAppServiceContract]
    public interface IAppManager
    {
        /// <summary>
        /// Initializes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task InitializeAppAsync(IAppContext appContext, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finalizes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task FinalizeAppAsync(IAppContext appContext, CancellationToken cancellationToken = default);
    }
}