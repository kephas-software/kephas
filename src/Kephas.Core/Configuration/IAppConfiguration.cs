// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfiguration.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Manager for application configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// Manager for application configuration.
    /// </summary>
    public interface IAppConfiguration : IIndexable
    {
        /// <summary>
        /// Gets the setting with the provided key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <remarks>If the setting is not found, returns <c>null</c>.</remarks>
        /// <returns>The setting with the provided key.</returns>
        object GetSetting(string key);

        /// <summary>
        /// Gets the settings with the provided pattern and returns an object representing these settings.
        /// </summary>
        /// <param name="searchPattern">A pattern specifying the settings to be retrieved.</param>
        /// <param name="settingsType">Type of the returned settings object. If not provided (<c>null</c>), an <see cref="Expando"/> object will be returned.</param>
        /// <returns>
        /// The settings.
        /// </returns>
        object GetSettings(string searchPattern, Type settingsType = null);
    }

    /// <summary>
    /// Extension methods for <see cref="IAppConfiguration"/>.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Gets the settings with the provided pattern and returns an object representing these settings.
        /// </summary>
        /// <typeparam name="TSettings">Type of the settings.</typeparam>
        /// <param name="appConfiguration">The configuration to act on.</param>
        /// <param name="searchPattern">A pattern specifying the settings to be retrieved.</param>
        /// <returns>
        /// The settings.
        /// </returns>
        public static TSettings GetSettings<TSettings>(this IAppConfiguration appConfiguration, string searchPattern)
        {
            Requires.NotNull(appConfiguration, nameof(appConfiguration));

            return (TSettings)appConfiguration.GetSettings(searchPattern, typeof(TSettings));
        }
    }
}