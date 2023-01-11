// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationAppServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Configuration;

    /// <summary>
    /// Extension methods for <see cref="IAppServiceCollection"/> related to configuration.
    /// </summary>
    public static class ConfigurationAppServiceCollectionExtensions
    {
        /// <summary>
        /// Sets the configuration store to The application services.
        /// </summary>
        /// <param name="appServices">The application services.</param>
        /// <param name="configurationStore">The configuration store.</param>
        /// <returns>
        /// This <paramref name="appServices"/>.
        /// </returns>
        public static IAppServiceCollection UseConfigurationStore(this IAppServiceCollection appServices, IConfigurationStore configurationStore)
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            configurationStore = configurationStore ?? throw new ArgumentNullException(nameof(configurationStore));

            appServices.Add(configurationStore);

            return appServices;
        }
    }
}