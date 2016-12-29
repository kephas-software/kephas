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
    }
}