// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPluginManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IPluginManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using Kephas.Plugins.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Interface for plugin manager.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IPluginManager
    {
        /// <summary>
        /// Installs the plugin asynchronously.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        Task<IOperationResult> InstallPluginAsync(PluginIdentity plugin, IContext context = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uninstalls the plugin asynchronously.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the uninstall plugin.
        /// </returns>
        Task<IOperationResult> UninstallPluginAsync(PluginIdentity plugin, IContext context = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the plugins in this collection.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the plugins in this collection.
        /// </returns>
        IEnumerable<IPlugin> GetInstalledPlugins();

        /// <summary>
        /// Gets available plugins asynchronously.
        /// </summary>
        /// <param name="filter">Specifies the filter.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the available plugins.
        /// </returns>
        Task<IEnumerable<IPluginInfo>> GetAvailablePluginsAsync(Action<ISearchContext> filter = null, CancellationToken cancellationToken = default);
    }
}
