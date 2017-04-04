// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    /// Shared service contract for the application manager.
    /// </summary>
    /// <remarks>
    /// The application manager is a service whose concern is to initialize and finalize the application.
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// var appManager = compositionContext.GetExport&lt;IAppManager&gt;();
    /// var appContext = new AppContext();
    /// await appManager.InitializeAppAsync(appContext);
    /// ...
    /// await appManager.FinalizeAppAsync(appContext);
    /// </code>
    /// </example>
    [SharedAppServiceContract]
    public interface IAppManager
    {
        /// <summary>
        /// Gets the application manifest.
        /// </summary>
        IAppManifest AppManifest { get; }

        /// <summary>
        /// Initializes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task InitializeAppAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Finalizes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task FinalizeAppAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken));
    }
}