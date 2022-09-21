// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the logging service collection extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Configuration
{
    using System;

    using Kephas.Configuration;
    using Kephas.Runtime;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Configuration extensions.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Uses the extensions configuration.
        /// </summary>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The provided ambient services.
        /// </returns>
        public static IAmbientServices UseConfiguration(this IAmbientServices ambientServices, IConfiguration configuration)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            ambientServices.Add<IConfigurationStore>(new ConfigurationStore(configuration, ambientServices.GetTypeRegistry()));

            return ambientServices;
        }
    }
}