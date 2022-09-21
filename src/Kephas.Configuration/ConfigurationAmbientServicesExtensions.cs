// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationAmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Configuration;

    /// <summary>
    /// Extension methods for <see cref="IAmbientServices"/> related to configuration.
    /// </summary>
    public static class ConfigurationAmbientServicesExtensions
    {
        /// <summary>
        /// Sets the configuration store to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="configurationStore">The configuration store.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices UseConfigurationStore(this IAmbientServices ambientServices, IConfigurationStore configurationStore)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            configurationStore = configurationStore ?? throw new ArgumentNullException(nameof(configurationStore));

            ambientServices.Add(configurationStore);

            return ambientServices;
        }
    }
}