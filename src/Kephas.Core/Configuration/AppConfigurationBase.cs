// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppConfigurationBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the configuration base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Resources;

    /// <summary>
    /// Base class for configurations.
    /// </summary>
    public abstract class AppConfigurationBase : IAppConfiguration
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
        public virtual object this[string key]
        {
            get
            {
                return this.GetSetting(key);
            }
            set
            {
                throw new NotSupportedException(Strings.ConfigurationBase_SettingValueNotSupported_Exception);
            }
        }

        /// <summary>
        /// Gets the setting with the provided key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <remarks>If the setting is not found, returns <c>null</c>.</remarks>
        /// <returns>The setting with the provided key.</returns>
        public virtual object GetSetting(string key)
        {
            var settings = this.GetSettingsCore(key);
            return settings.FirstOrDefault().Value;
        }

        /// <summary>
        /// Gets the settings with the provided pattern and returns an object representing these settings.
        /// </summary>
        /// <param name="searchPattern">A pattern specifying the settings to be retrieved.</param>
        /// <param name="settingsType">Type of the returned settings object (optional). If not provided (<c>null</c>), an <see cref="Expando"/> object will be returned.</param>
        /// <returns>
        /// The settings.
        /// </returns>
        public virtual object GetSettings(string searchPattern, Type settingsType = null)
        {
            var settingsList = this.GetSettingsCore(searchPattern);
            if (settingsType == typeof(IIndexable) || settingsType == typeof(IExpando) || settingsType == typeof(Expando))
            {
                settingsType = null;
            }

            var settings = settingsType?.AsRuntimeTypeInfo().CreateInstance();
            var expando = settings == null ? new Expando() : new Expando(settings);

            // TODO implement a better value setting, taking into consideration the "dot syntax"
            foreach (var setting in settingsList)
            {
                expando[setting.Key] = setting.Value;
            }

            return settings ?? expando;
        }

        /// <summary>
        /// Gets all available settings for the specified search pattern.
        /// </summary>
        /// <param name="searchPattern">A pattern specifying the settings to search for (optional).</param>
        /// <returns>
        /// An enumeration of settings.
        /// </returns>
        protected abstract IEnumerable<KeyValuePair<string, object>> GetSettingsCore(string searchPattern);
    }
}