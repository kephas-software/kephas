// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspNetConfigurationStore.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ASP net configuration store class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Configuration
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Configuration;
    using Kephas.Dynamic;

    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// A configuration store based on the extensions configuration.
    /// </summary>
    public class ConfigurationStore : IConfigurationStore
    {
        private readonly IConfiguration configuration;
        private IDictionary<string, object> settingsMap = new Dictionary<string, object>();


        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationStore"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public ConfigurationStore(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

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
        public object this[string key]
        {
            get => this.configuration[key];
            set => this.SetTypedValue(key, value, syncSettings: true);
        }

        /// <summary>
        /// Configures the settings.
        /// </summary>
        /// <typeparam name="TSettings">Type of the settings.</typeparam>
        /// <param name="optionsConfig">The options configuration.</param>
        public void Configure<TSettings>(Action<TSettings> optionsConfig)
            where TSettings : class, new()
        {
            if (!this.settingsMap.TryGetValue(typeof(TSettings).FullName, out var settings))
            {
                settings = new TSettings();
                this.settingsMap.Add(typeof(TSettings).FullName, settings);
            }

            optionsConfig?.Invoke((TSettings)settings);

            this.SetTypedValue(typeof(TSettings).FullName, settings, syncSettings: false);
        }

        /// <summary>
        /// Tries to get the indicated settings.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>
        /// The required settings or <c>null</c>.
        /// </returns>
        public object TryGetSettings(Type settingsType)
        {
            if (settingsType == null)
            {
                return null;
            }

            return this.settingsMap.TryGetValue(settingsType.FullName);
        }

        private void SetTypedValue(string key, object value, bool syncSettings)
        {
            // TODO synchronize with the settingsMap dictionary
            this.SetValue(key, value);
        }

        private void SetValue(string key, object value)
        {
            if (value == null)
            {
                this.configuration[key] = null;
                return;
            }

            if (value is string stringValue)
            {
                this.configuration[key] = stringValue;
                return;
            }

            var valueType = value.GetType();
            if (valueType.IsValueType)
            {
                this.configuration[key] = value.ToString();
                return;
            }

            var flattenedValue = new Expando(value).ToDictionary();
            foreach (var keyValue in flattenedValue)
            {
                this.SetValue($"{key}:{keyValue.Key}", keyValue.Value);
            }
        }
    }
}