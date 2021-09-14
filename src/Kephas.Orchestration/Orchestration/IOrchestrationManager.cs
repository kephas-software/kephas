// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrchestrationManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOrchestrationManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application.Configuration;
    using Kephas.Application.Reflection;
    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// Interface for orchestration manager.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IOrchestrationManager
    {
        /// <summary>
        /// Gets the root application instance ID.
        /// </summary>
        /// <returns>The root application instance ID.</returns>
        string GetRootAppInstanceId();

        /// <summary>
        /// Gets the application settings for the provided application.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <returns>The application settings.</returns>
        AppSettings? GetAppSettings(string appId);

        /// <summary>
        /// Gets the live apps asynchronously.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the live apps.
        /// </returns>
        Task<IEnumerable<IRuntimeAppInfo>> GetLiveAppsAsync(Action<IContext>? optionsConfig = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts the application asynchronously.
        /// </summary>
        /// <param name="appInfo">Information describing the application.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields an operation result.
        /// </returns>
        Task<IOperationResult> StartAppAsync(IAppInfo appInfo, IDynamic arguments, Action<IContext>? optionsConfig = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stops a running application asynchronously.
        /// </summary>
        /// <param name="runtimeAppInfo">Information describing the runtime application.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields an operation result.
        /// </returns>
        Task<IOperationResult> StopAppAsync(IRuntimeAppInfo runtimeAppInfo, Action<IContext>? optionsConfig = null, CancellationToken cancellationToken = default);
    }
}