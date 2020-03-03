// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetInstalledPluginsHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the get installed plugins message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Licensing;
using Kephas.Services;

namespace Kephas.Plugins.Endpoints
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Messaging;
    using Kephas.Plugins;

    /// <summary>
    /// A get installed plugins message handler.
    /// </summary>
    public class GetInstalledPluginsHandler : MessageHandlerBase<GetInstalledPluginsMessage, GetInstalledPluginsResponseMessage>
    {
        private readonly IPluginManager pluginManager;
        private readonly ILicensingManager licensingManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetInstalledPluginsHandler"/> class.
        /// </summary>
        /// <param name="pluginManager">Manager for plugins.</param>
        /// <param name="licensingManager">The licensing manager.</param>
        public GetInstalledPluginsHandler(IPluginManager pluginManager, ILicensingManager licensingManager)
        {
            this.pluginManager = pluginManager;
            this.licensingManager = licensingManager;
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
        public override async Task<GetInstalledPluginsResponseMessage> ProcessAsync(GetInstalledPluginsMessage message, IMessagingContext context, CancellationToken token)
        {
            var plugins = this.pluginManager.GetInstalledPlugins();

            return new GetInstalledPluginsResponseMessage
            {
                Plugins = plugins.Select(p => this.GetPluginData(p, context)).ToArray(),
            };
        }

        private PluginData GetPluginData(IPlugin plugin, IContext context)
        {
            var licenceCheckResult = this.licensingManager.CheckLicense(plugin.Identity, context);
            return new PluginData
            {
                Id = plugin.Identity.Id,
                Version = plugin.Identity.Version,
                Location = plugin.Location,
                State = plugin.State,
                IsLicensed = licenceCheckResult.IsLicensed,
                LicenseCheckMessage = licenceCheckResult.Messages?.FirstOrDefault()?.Message,
                License = this.licensingManager.GetLicense(plugin.Identity, context),
            };
        }
    }
}
