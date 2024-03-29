﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetInstalledPluginsHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the get installed plugins message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Endpoints
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Licensing;
    using Kephas.Messaging;
    using Kephas.Plugins;
    using Kephas.Services;

    /// <summary>
    /// A get installed plugins message handler.
    /// </summary>
    public class
        GetInstalledPluginsHandler : IMessageHandler<GetInstalledPluginsMessage, GetInstalledPluginsResponse>
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
        public async Task<GetInstalledPluginsResponse> ProcessAsync(
            GetInstalledPluginsMessage message,
            IMessagingContext context,
            CancellationToken token)
        {
            var plugins = this.pluginManager.GetInstalledPlugins();

            return new GetInstalledPluginsResponse
            {
                Plugins = plugins.Select(p => this.GetPluginData(p, context, message.IncludeLicense)).ToArray(),
            };
        }

        private PluginData GetPluginData(IPlugin plugin, IContext context, bool includeLicense)
        {
            var pluginData = includeLicense ? new LicensePluginData() : new PluginData();
            pluginData.Id = plugin.Identity.Id;
            pluginData.Version = plugin.Identity.Version?.ToString();
            pluginData.State = plugin.State;
            pluginData.Location = plugin.Location;

            if (pluginData is LicensePluginData licensePluginData)
            {
                var licenceCheckResult = this.licensingManager.CheckLicense(plugin.Identity, context);
                licensePluginData.IsLicensed = licenceCheckResult.Value;
                licensePluginData.LicenseCheckMessage = licenceCheckResult.Messages?.FirstOrDefault()?.Message;
                licensePluginData.License = this.licensingManager.GetLicense(plugin.Identity, context);
            }

            return pluginData;
        }
    }
}