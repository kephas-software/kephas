// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ambient services extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Configuration;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Runtime;

    /// <summary>
    /// Extension methods for <see cref="IAmbientServices"/>.
    /// </summary>
    public static class AmbientServicesExtensions
    {
        /// <summary>
        /// Gets the runtime type registry.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// The runtime type registry.
        /// </returns>
        public static IRuntimeTypeRegistry GetTypeRegistry(this IAmbientServices ambientServices) =>
            (ambientServices ?? throw new ArgumentNullException(nameof(ambientServices))).GetRequiredService<IRuntimeTypeRegistry>();

        /// <summary>
        /// Configures the settings.
        /// </summary>
        /// <typeparam name="TSettings">Type of the settings.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="optionsConfig">The options configuration.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Configure<TSettings>(this IAmbientServices ambientServices, Action<TSettings> optionsConfig)
            where TSettings : class, new()
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            ambientServices.GetRequiredService<IConfigurationStore>().Configure(optionsConfig);
            return ambientServices;
        }

        /// <summary>
        /// Sets the configuration store to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="configurationStore">The configuration store.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithConfigurationStore(this IAmbientServices ambientServices, IConfigurationStore configurationStore)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            Requires.NotNull(configurationStore, nameof(configurationStore));

            ambientServices.Register(configurationStore);

            return ambientServices;
        }
    }
}