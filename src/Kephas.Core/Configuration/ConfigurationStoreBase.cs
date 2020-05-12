// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationStoreBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the configuration store base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Diagnostics.Contracts;

namespace Kephas.Configuration
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Dynamic;
    using Kephas.Reflection;

    /// <summary>
    /// Abstract base class for configuration stores.
    /// </summary>
    public abstract class ConfigurationStoreBase : IConfigurationStore
    {
        private IDictionary<Type, object> settingsMap = new Dictionary<Type, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationStoreBase"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        protected ConfigurationStoreBase(IIndexable store)
        {
            this.InternalStore = store;
        }

        /// <summary>
        /// Gets the internal store.
        /// </summary>
        /// <value>
        /// The internal store.
        /// </value>
        protected IIndexable InternalStore { get; }

        /// <summary>
        /// Indexer to get or set items within this collection using array index syntax.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The indexed item.
        /// </returns>
        public object? this[string key]
        {
            get => this.GetValue(key);
            set => this.SetValue(key, value, syncSettingsMap: true);
        }

        /// <summary>
        /// Configures the settings.
        /// </summary>
        /// <typeparam name="TSettings">Type of the settings.</typeparam>
        /// <param name="optionsConfig">The options configuration.</param>
        public virtual void Configure<TSettings>(Action<TSettings>? optionsConfig)
            where TSettings : class, new()
        {
            if (optionsConfig == null)
            {
                return;
            }

            var settingsType = typeof(TSettings);

            var settings = this.TryGetOrAddSettings(typeof(TSettings), () => new TSettings());
            if (settings != null)
            {
                optionsConfig.Invoke((TSettings)settings);
                this.SetValue(typeof(TSettings).FullName, settings, syncSettingsMap: false);
            }
        }

        /// <summary>
        /// Tries to get the indicated settings.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>
        /// The required settings or <c>null</c>.
        /// </returns>
        public object? TryGetSettings(Type settingsType)
        {
            return this.TryGetOrAddSettings(settingsType, () => settingsType.AsRuntimeTypeInfo().CreateInstance());
        }

        /// <summary>
        /// Updates the settings.
        /// </summary>
        /// <param name="settings">The settings to be updated.</param>
        public void UpdateSettings(object settings)
        {
            Requires.NotNull(settings, nameof(settings));

            this.TryAddOrUpdateSettings(settings.GetType(), settings);
        }

        /// <summary>
        /// Tries to get the indicated settings and, if not found, add a new instance.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <param name="ctor">The constructor used to create the settings, if not found.</param>
        /// <returns>
        /// The required settings or <c>null</c>.
        /// </returns>
        protected virtual object? TryGetOrAddSettings(Type settingsType, Func<object> ctor)
        {
            if (settingsType == null)
            {
                return null;
            }

            var settings = this.settingsMap.TryGetValue(settingsType);
            if (settings != null)
            {
                return settings;
            }

            lock (this.settingsMap)
            {
                settings = this.settingsMap.TryGetValue(settingsType);
                if (settings != null)
                {
                    return settings;
                }

                this.settingsMap.Add(settingsType, settings = ctor());
                return settings;
            }
        }

        /// <summary>
        /// Tries to get the indicated settings and, if not found, add a new instance.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// True if the add or update was successful, false otherwise.
        /// </returns>
        protected virtual bool TryAddOrUpdateSettings(Type settingsType, object settings)
        {
            if (settingsType == null)
            {
                return false;
            }

            lock (this.settingsMap)
            {
                this.settingsMap[settingsType] = settings;
                return true;
            }
        }

        /// <summary>
        /// Creates the settings of the provided type.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>
        /// The new settings.
        /// </returns>
        protected virtual object CreateSettings(Type settingsType)
        {
            var settingsTypeInfo = settingsType.AsRuntimeTypeInfo();
            var settings = settingsTypeInfo.CreateInstance();

            return settings;
        }

        /// <summary>
        /// Gets the aggregated value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The aggregated value.
        /// </returns>
        protected virtual object? GetValue(string key)
        {
            return this.InternalStore[key];
        }

        /// <summary>
        /// Sets a flattened value, indicating whether to synchronize the settings map.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="syncSettingsMap">True to synchronise the settings map.</param>
        protected virtual void SetValue(string key, object? value, bool syncSettingsMap)
        {
            // TODO synchronize with the settingsMap dictionary
            this.InternalStore[key] = value;
        }
    }
}
