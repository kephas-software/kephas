// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginsServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ambient services plugins extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Plugins.Application;
    using Kephas.Services.Builder;

    /// <summary>
    /// The plugins ambient services builder extensions.
    /// </summary>
    public static class PluginsServicesExtensions
    {
        /// <summary>
        /// Sets the plugins-enabled application runtime to The application services.
        /// </summary>
        /// <param name="servicesBuilder">The application services.</param>
        /// <param name="settingsConfig">Optional. The settings configuration.</param>
        /// <param name="runtimeConfig">Optional. The post configuration for the <see cref="PluginsAppRuntime"/>.</param>
        /// <returns>
        /// The <paramref name="servicesBuilder"/>.
        /// </returns>
        public static IAppServiceCollectionBuilder WithPluginsAppRuntime(
            this IAppServiceCollectionBuilder servicesBuilder,
            Action<PluginsAppRuntimeSettings>? settingsConfig = null,
            Action<PluginsAppRuntime>? runtimeConfig = null)
        {
            servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));

            var settings = new PluginsAppRuntimeSettings();
            settingsConfig?.Invoke(settings);

            var appRuntime = new PluginsAppRuntime(settings);
            runtimeConfig?.Invoke(appRuntime);

            return servicesBuilder.WithAppRuntime(appRuntime);
        }
    }
}
