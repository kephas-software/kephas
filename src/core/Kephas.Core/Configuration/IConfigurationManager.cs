// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Manager for application configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Manager for application configuration.
    /// </summary>
    [ContractClass(typeof(ConfigurationManagerContractClass))]
    public interface IConfigurationManager
    {
        /// <summary>
        /// Gets the setting with the provided key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <remarks>If the setting is not found, returns <c>null</c>.</remarks>
        /// <returns>The setting with the provided key.</returns>
        string GetSetting(string key);

        /// <summary>
        /// Gets the service settings.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <returns>The settings for the provided service type.</returns>
        TSettings GetServiceSettings<TService, TSettings>();

        /// <summary>
        /// Gets the service settings.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>The settings for the provided service type.</returns>
        object GetServiceSettings(Type serviceType);
    }

    /// <summary>
    /// Contract class for <see cref="IConfigurationManager"/>.
    /// </summary>
    [ContractClassFor(typeof(IConfigurationManager))]
    internal abstract class ConfigurationManagerContractClass : IConfigurationManager
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
            Contract.Requires(!string.IsNullOrEmpty(key));
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
            Contract.Requires(serviceType != null);
            return null;
        }
    }
}