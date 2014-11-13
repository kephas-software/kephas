// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugAppConfigurationManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Configuration manager for debugging.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Diagnostics.Logging
{
    using System;

    using Kephas.Configuration;

    /// <summary>
    /// Configuration manager for debugging.
    /// </summary>
    public class DebugConfigurationManager : IConfigurationManager
    {
        /// <summary>
        /// Gets the setting with the provided key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The setting with the provided key.
        /// </returns>
        /// <remarks>
        /// If the setting is not found, returns <c>null</c>.
        /// </remarks>
        public string GetSetting(string key)
        {
            return null;
        }

        /// <summary>
        /// Gets the service settings.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <returns>
        /// The settings for the provided service type.
        /// </returns>
        public TSettings GetServiceSettings<TService, TSettings>()
        {
            return default(TSettings);
        }

        /// <summary>
        /// Gets the service settings.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        /// The settings for the provided service type.
        /// </returns>
        public object GetServiceSettings(Type serviceType)
        {
            return null;
        }
    }
}