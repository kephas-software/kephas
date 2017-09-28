// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppConfigurationProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application configuration provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration.Providers
{
    using System;

    using Kephas.Application.Configuration;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Configuration provider from application settings.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    [ProcessingPriority(Priority.Lowest)]
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
            return this.AppConfiguration.GetSettings(settingsType, this.GetSectionName(settingsType));
        }

        /// <summary>
        /// Gets the section name out of the provided settings type.
        /// </summary>
        /// <remarks>
        /// The section name is computed the name of the type without the 'Settings' ending,
        /// using the camel case convention.
        /// </remarks>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>
        /// The section name.
        /// </returns>
        protected virtual string GetSectionName(Type settingsType)
        {
            return AppConfigurationExtensions.GetSettingsSectionName(settingsType);
        }
    }
}