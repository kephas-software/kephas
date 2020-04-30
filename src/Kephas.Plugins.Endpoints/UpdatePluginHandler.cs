// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdatePluginHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the update plugin message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Endpoints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Operations;
    using Kephas.Plugins;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An update plugin message handler.
    /// </summary>
    public class UpdatePluginHandler : MessageHandlerBase<UpdatePluginMessage, ResponseMessage>
    {
        private readonly IPluginManager pluginManager;
        private readonly IAppContext appContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePluginHandler"/> class.
        /// </summary>
        /// <param name="pluginManager">Manager for plugins.</param>
        /// <param name="appContext">Context for the application.</param>
        public UpdatePluginHandler(IPluginManager pluginManager, IAppContext appContext)
        {
            this.pluginManager = pluginManager;
            this.appContext = appContext;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public override async Task<ResponseMessage> ProcessAsync(UpdatePluginMessage message, IMessagingContext context, CancellationToken token)
        {
            var toUpdate = await this.GetPackagesToUpdateAsync(message, token).PreserveThreadContext();

            if (toUpdate.Count == 0)
            {
                this.appContext.Logger.Info("No packages to update, all have the the requested version.");

                return new ResponseMessage
                {
                    Message = "No packages to update, all have the the requested version.",
                };
            }

            var successful = 0;
            var failed = 0;
            foreach (var pluginIdentity in toUpdate)
            {
                var result = await this.UpdatePluginAsync(pluginIdentity, context, token)
                    .PreserveThreadContext();
                if (result.OperationState == OperationState.Failed)
                {
                    failed++;
                }
                else
                {
                    successful++;
                }
            }

            return new ResponseMessage
            {
                Message = successful == 0
                    ? $"{failed} failed updates."
                    : failed == 0
                        ? $"{successful} successful updates."
                        : $"{successful} successful updates, {failed} failed updates.",
            };
        }

        private async Task<List<AppIdentity>> GetPackagesToUpdateAsync(UpdatePluginMessage message, CancellationToken token)
        {
            List<AppIdentity> toUpdate;
            if (message.Id.Equals("all", StringComparison.InvariantCultureIgnoreCase))
            {
                var installedPlugins = this.pluginManager.GetInstalledPlugins().ToList();

                if (UpdatePluginMessage.LatestVersion.Equals(message.Version, StringComparison.InvariantCultureIgnoreCase))
                {
                    var availablePackages = (await this.pluginManager.GetAvailablePluginsAsync(
                        s => s.Take(installedPlugins.Count).IncludePrerelease(message.IncludePrerelease),
                        token).PreserveThreadContext()).Value;
                    toUpdate = installedPlugins
                        .Select(p => (plugin: p, version: availablePackages
                            .FirstOrDefault(pkg =>
                                pkg.Identity.Id.Equals(p.Identity.Id, StringComparison.InvariantCultureIgnoreCase))
                            ?.Identity.Version))
                        .Where(tuple =>
                            !string.IsNullOrEmpty(tuple.version) && !tuple.plugin.Identity.Version.Equals(
                                tuple.version,
                                StringComparison.InvariantCultureIgnoreCase))
                        .Select(tuple => new AppIdentity(tuple.plugin.Identity.Id, tuple.version))
                        .ToList();
                }
                else
                {
                    toUpdate = installedPlugins.Where(p =>
                            !p.Identity.Version.Equals(message.Version, StringComparison.InvariantCultureIgnoreCase))
                        .Select(p => new AppIdentity(p.Identity.Id, message.Version)).ToList();
                }
            }
            else
            {
                if (UpdatePluginMessage.LatestVersion.Equals(message.Version, StringComparison.InvariantCultureIgnoreCase))
                {
                    var availablePackage = (await this.pluginManager.GetAvailablePluginsAsync(
                            s => s.SearchTerm(message.Id).IncludePrerelease(message.IncludePrerelease),
                            token).PreserveThreadContext()).Value
                        .FirstOrDefault(p =>
                            p.Identity.Id.Equals(message.Id, StringComparison.InvariantCultureIgnoreCase));
                    toUpdate = availablePackage == null
                        ? new List<AppIdentity>()
                        : new List<AppIdentity> { availablePackage.Identity };
                }
                else
                {
                    toUpdate = new List<AppIdentity> { new AppIdentity(message.Id, message.Version) };
                }
            }

            return toUpdate;
        }

        private async Task<IOperationResult> UpdatePluginAsync(AppIdentity pluginIdentity, IMessagingContext context, CancellationToken token)
        {
            try
            {
                this.appContext.Logger.Info(
                    "Updating plugin {plugin} to version {version}...",
                    pluginIdentity.Id,
                    pluginIdentity.Version);

                var result = await this.pluginManager
                    .UpdatePluginAsync(pluginIdentity, ctx => ctx.Merge(context), token)
                    .PreserveThreadContext();

                var plugin = result.Value;
                var pluginId = plugin?.GetTypeInfo().Name ?? pluginIdentity.Id;
                var pluginVersion = plugin?.GetTypeInfo().Identity.Version ?? pluginIdentity.Version;

                this.appContext.Logger.Info(
                    "Plugin {plugin} updated to version {version} in {pluginPath}. Elapsed: {elapsed:c}.",
                    pluginId,
                    pluginVersion,
                    plugin?.Location,
                    result.Elapsed);
                return result.MergeMessage(
                    $"Plugin {pluginId} updated to version {pluginVersion} in {plugin?.Location}. Elapsed: {result.Elapsed:c}.");
            }
            catch (Exception ex)
            {
                this.appContext.Logger.Error(
                    ex,
                    "Plugin {plugin} could not be updated to version {version}.",
                    pluginIdentity.Id,
                    pluginIdentity.Version);
                return new OperationResult<IPlugin>()
                    .MergeMessage(
                        $"Plugin {pluginIdentity.Id} could not be updated to version {pluginIdentity.Version}.")
                    .Complete(TimeSpan.Zero, OperationState.Failed);
            }
        }
    }
}