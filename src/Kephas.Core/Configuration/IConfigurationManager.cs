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

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// Manager for application configuration.
    /// </summary>
    [ContractClass(typeof(ConfigurationManagerContractClass))]
    public interface IConfigurationManager : IIndexable
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
    /// Contract class for <see cref="IConfigurationManager"/>.
    /// </summary>
    [ContractClassFor(typeof(IConfigurationManager))]
    internal abstract class ConfigurationManagerContractClass : IConfigurationManager
    {
        /// <summary>
        /// Convenience method that provides a string Indexer
        /// to the Properties collection AND the strongly typed
        /// properties of the object by name.
        /// // dynamic
        /// exp["Address"] = "112 nowhere lane";
        /// // strong
        /// var name = exp["StronglyTypedProperty"] as string;.
        /// </summary>
        /// <value>
        /// The <see cref="object" /> identified by the key.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The requested property value.</returns>
        public abstract object this[string key] { get; set; }

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
        public object GetSetting(string key)
        {
            Contract.Requires(!string.IsNullOrEmpty(key));
            return null;
        }

        /// <summary>
        /// Gets the settings with the provided pattern and returns an object representing these settings.
        /// </summary>
        /// <param name="searchPattern">A pattern specifying the settings to be retrieved.</param>
        /// <param name="settingsType">Type of the returned settings object. If not provided, and <see cref="Expando"/> object will be returned.</param>
        /// <returns>
        /// The settings.
        /// </returns>
        public object GetSettings(string searchPattern, Type settingsType)
        {
            return null;
        }
    }

    /// <summary>
    /// Extension methods for <see cref="IConfigurationManager"/>.
    /// </summary>
    public static class ConfigurationManagerExtensions
    {
        /// <summary>
        /// Gets the settings with the provided pattern and returns an object representing these settings.
        /// </summary>
        /// <typeparam name="TSettings">Type of the settings.</typeparam>
        /// <param name="configurationManager">The configurationManager to act on.</param>
        /// <param name="searchPattern">A pattern specifying the settings to be retrieved.</param>
        /// <returns>
        /// The settings.
        /// </returns>
        public static TSettings GetSettings<TSettings>(this IConfigurationManager configurationManager, string searchPattern)
        {
            Requires.NotNull(configurationManager, nameof(configurationManager));

            return (TSettings)configurationManager.GetSettings(searchPattern, typeof(TSettings));
        }
    }
}