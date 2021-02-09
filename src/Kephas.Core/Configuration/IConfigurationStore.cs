// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationStore.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IConfigurationStore interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System;

    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// Contract interface for the configuration store.
    /// </summary>
    /// <remarks>
    /// The configuration store is used to store configuration values based on keys.
    /// </remarks>
    public interface IConfigurationStore : IIndexable
    {
        /// <summary>
        /// Configures the settings.
        /// </summary>
        /// <typeparam name="TSettings">Type of the settings.</typeparam>
        /// <param name="optionsConfig">The options configuration.</param>
        void Configure<TSettings>(Action<TSettings> optionsConfig)
            where TSettings : class, new();

        /// <summary>
        /// Tries to get the indicated settings.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The required settings or <c>null</c>.
        /// </returns>
        object? TryGetSettings(Type settingsType, IContext? context);

        /// <summary>
        /// Updates the settings.
        /// </summary>
        /// <param name="settings">The settings to be updated.</param>
        /// <param name="context">The context.</param>
        void UpdateSettings(object settings, IContext? context);
    }
}