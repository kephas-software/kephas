// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppConfiguration.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Manager for application configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Configuration
{
    using System;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Reflection;

    /// <summary>
    /// Provides the application configuration as an expando object.
    /// </summary>
    /// <remarks>
    /// The members of the application configuration are the sections.
    /// </remarks>
    public interface IAppConfiguration : IExpando
    {
    }

    /// <summary>
    /// Extension methods for <see cref="IAppConfiguration"/>.
    /// </summary>
    public static class AppConfigurationExtensions
    {
        /// <summary>
        /// Gets the 'appSettings' section.
        /// </summary>
        /// <param name="appConfiguration">The application configuration.</param>
        /// <returns>
        /// The application settings.
        /// </returns>
        public static IExpando GetAppSettings(this IAppConfiguration appConfiguration)
        {
            Requires.NotNull(appConfiguration, nameof(appConfiguration));

            return appConfiguration[AppConfigurationSections.AppSettings]?.ToExpando();
        }

        /// <summary>
        /// Gets the 'appSettings' section mapped to the <typeparamref name="TSettings"/> type.
        /// </summary>
        /// <typeparam name="TSettings">Type of the settings.</typeparam>
        /// <param name="appConfiguration">The application configuration.</param>
        /// <returns>
        /// The application settings.
        /// </returns>
        public static TSettings GetAppSettings<TSettings>(this IAppConfiguration appConfiguration)
        {
            Requires.NotNull(appConfiguration, nameof(appConfiguration));

            return (TSettings)GetSettings(appConfiguration, typeof(TSettings), AppConfigurationSections.AppSettings);
        }

        /// <summary>
        /// Gets the settings as a typed object.
        /// </summary>
        /// <param name="appConfiguration">The application configuration.</param>
        /// <param name="settingsType">Type of the settings.</param>
        /// <param name="sectionName">Name of the section from where the settings sould be retrieved
        ///                           (optional). If not provided, it is considered the name of the
        ///                           type without the 'Settings' ending.</param>
        /// <returns>
        /// The settings as a typed object.
        /// </returns>
        public static object GetSettings(this IAppConfiguration appConfiguration, Type settingsType, string sectionName = null)
        {
            Requires.NotNull(appConfiguration, nameof(appConfiguration));
            Requires.NotNull(settingsType, nameof(settingsType));

            var settingsTypeInfo = settingsType.AsRuntimeTypeInfo();
            sectionName = string.IsNullOrEmpty(sectionName) ? GetSettingsSectionName(settingsType) : sectionName;
            var rawSettings = appConfiguration[sectionName];
            if (rawSettings == null)
            {
                return settingsTypeInfo.DefaultValue;
            }

            if (settingsTypeInfo.TypeInfo.IsAssignableFrom(rawSettings.GetType().GetTypeInfo()))
            {
                return rawSettings;
            }

            // copy the values from the raw settings into the typed settings.
            var settings = settingsTypeInfo.CreateInstance();
            var indexableSettings = settings.ToExpando();
            var rawDictionary = rawSettings.ToExpando().ToDictionary();
            foreach (var keyValue in rawDictionary)
            {
                indexableSettings[keyValue.Key] = keyValue.Value;
            }

            return settings;
        }

        /// <summary>
        /// Gets the settings as a typed object.
        /// </summary>
        /// <typeparam name="TSettings">Type of the settings.</typeparam>
        /// <param name="appConfiguration">The configuration to act on.</param>
        /// <param name="sectionName">Name of the section from where the settings sould be retrieved (optional).
        ///                           If not provided, it is considered the name of the type without the 'Settings' ending.</param>
        /// <returns>
        /// The settings as a typed object.
        /// </returns>
        public static TSettings GetSettings<TSettings>(this IAppConfiguration appConfiguration, string sectionName = null)
        {
            return (TSettings)GetSettings(appConfiguration, typeof(TSettings), sectionName);
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
        public static string GetSettingsSectionName(Type settingsType)
        {
            const string SettingsEnding = "Settings";
            var sectionName = settingsType.Name;
            if (sectionName.EndsWith(SettingsEnding) && sectionName.Length > SettingsEnding.Length)
            {
                sectionName = sectionName.Substring(0, sectionName.Length - SettingsEnding.Length);
            }

            sectionName = sectionName.ToCamelCase();

            return sectionName;
        }
    }
}