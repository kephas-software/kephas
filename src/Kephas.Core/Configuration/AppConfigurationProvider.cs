// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppConfigurationProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application configuration provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Configuration provider from application settings.
    /// </summary>
    [OverridePriority(Priority.Low)]
    [ProcessingPriority(Priority.Low)]
    public class AppConfigurationProvider : IConfigurationProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigurationProvider"/> class.
        /// </summary>
        /// <param name="appConfiguration">The application configuration.</param>
        public AppConfigurationProvider(IAppConfiguration appConfiguration)
        {
            Requires.NotNull(appConfiguration, nameof(appConfiguration));

            this.AppConfiguration = appConfiguration;
        }

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        /// <value>
        /// The application configuration.
        /// </value>
        public IAppConfiguration AppConfiguration { get; }

        /// <summary>
        /// Gets the settings with the provided type.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>
        /// The settings.
        /// </returns>
        public virtual object GetSettings(Type settingsType)
        {
            Requires.NotNull(settingsType, nameof(settingsType));

            return this.AppConfiguration.GetSettings(this.GetSettingsPattern(settingsType), settingsType);
        }

        /// <summary>
        /// Gets the settings pattern.
        /// </summary>
        /// <remarks>
        /// By default, it returns the section having the same name as the settings type name and the
        /// settings having the name starting with the settings type name.
        /// </remarks>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>
        /// The settings pattern.
        /// </returns>
        protected virtual string GetSettingsPattern(Type settingsType)
        {
            var settingsTypeName = settingsType.Name.ToCamelCase();
            return $":{settingsTypeName}:*;{settingsTypeName}*";
        }
    }
}