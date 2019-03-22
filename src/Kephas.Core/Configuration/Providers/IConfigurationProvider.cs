// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IConfigurationProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration.Providers
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Interface for configuration provider.
    /// </summary>
    [SharedAppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(SettingsTypeAttribute) })]
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Gets the settings with the provided type.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>
        /// The settings.
        /// </returns>
        object GetSettings(Type settingsType);
    }

    /// <summary>
    /// Extension methods for <see cref="IConfigurationProvider"/>.
    /// </summary>
    public static class ConfigurationProviderExtensions
    {
        /// <summary>
        /// Gets the settings with the provided type.
        /// </summary>
        /// <typeparam name="T">Type of the settings.</typeparam>
        /// <param name="configurationProvider">The configurationProvider to act on.</param>
        /// <returns>
        /// The settings.
        /// </returns>
        public static T GetSettings<T>(this IConfigurationProvider configurationProvider)
        {
            Requires.NotNull(configurationProvider, nameof(configurationProvider));

            return (T)configurationProvider.GetSettings(typeof(T));
        }
    }
}