// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugin class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using System;
    using System.Collections.Generic;

    using Kephas;
    using Kephas.Application;
    using Kephas.Application.Reflection;
    using Kephas.Data;
    using Kephas.Dynamic;
    using Kephas.Plugins.Reflection;
    using Kephas.Reflection;

    /// <summary>
    /// A plugin instance.
    /// </summary>
    public class Plugin : Expando, IPlugin
    {
        private readonly IAppInfo pluginInfo;
        private readonly PluginData? pluginData;
        private readonly IPluginStore pluginStore;
        private readonly IAppRuntime appRuntime;
        private PluginState? state;
        private string? location;

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        /// <param name="pluginInfo">Information describing the plugin.</param>
        /// <param name="pluginData">Optional. Information describing the plugin.</param>
        internal Plugin(PluginInfo pluginInfo, PluginData? pluginData = null)
        {
            pluginInfo = pluginInfo ?? throw new System.ArgumentNullException(nameof(pluginInfo));

            this.pluginInfo = pluginInfo;
            this.pluginData = pluginData;
            this.pluginStore = pluginInfo.PluginStore;
            this.appRuntime = pluginInfo.AppRuntime;
        }

        /// <summary>
        /// Gets or sets the full pathname of the installation folder.
        /// </summary>
        /// <value>
        /// The full pathname of the installation folder.
        /// </value>
        public string? Location
        {
            get => this.location ?? this.appRuntime.GetAppLocation(this.Identity, throwOnNotFound: false);
            protected internal set => this.location = value;
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public PluginState State
        {
            get => this.state ?? this.GetPluginData().State;
            protected internal set => this.state = value;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        object IIdentifiable.Id => this.Identity.ToString();

        /// <summary>
        /// Gets the identity.
        /// </summary>
        /// <returns>
        /// The identity.
        /// </returns>
        public AppIdentity Identity => this.pluginInfo.Identity;

        /// <summary>
        /// Gets the type information for this instance.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        public IAppInfo GetTypeInfo() => this.pluginInfo;

        /// <summary>
        /// Gets type information.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        ITypeInfo IInstance.GetTypeInfo() => this.GetTypeInfo();

        /// <summary>
        /// Gets the plugin data.
        /// </summary>
        /// <returns>
        /// The plugin data.
        /// </returns>
        public virtual PluginData GetPluginData()
        {
            return this.pluginData ?? this.pluginStore.GetPluginData(this.Identity);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.pluginInfo.Identity} ({this.State})";
        }

        /// <summary>
        /// Converts the expando to a dictionary having as keys the property names and as values the
        /// respective properties' values.
        /// </summary>
        /// <param name="keyFunc">Optional. The key transformation function (optional).</param>
        /// <param name="valueFunc">Optional. The value transformation function (optional).</param>
        /// <returns>
        /// A dictionary of property values with their associated names.
        /// </returns>
        public override IDictionary<string, object?> ToDictionary(Func<string, string>? keyFunc = null, Func<object?, object?>? valueFunc = null)
        {
            var dictionary = base.ToDictionary(keyFunc, valueFunc);
            var data = this.GetPluginData().Data;
            foreach (var kv in data)
            {
                dictionary[kv.Key] = kv.Value;
            }

            return dictionary;
        }

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>
        /// A sequence that contains dynamic member names.
        /// </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var keyHash = new HashSet<string>();
            foreach (var name in base.GetDynamicMemberNames())
            {
                keyHash.Add(name);
                yield return name;
            }

            var data = this.GetPluginData().Data;
            foreach (var key in data.Keys)
            {
                if (!keyHash.Add(key))
                {
                    continue;
                }

                yield return key;
            }
        }

        /// <summary>
        /// Attempts to get the dynamic value with the given key.
        /// If the value is not found in the dynamic values, try to get it
        /// from the repository.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">[out] The value to get.</param>
        /// <returns>
        /// <c>true</c> if a value is found, <c>false</c> otherwise.
        /// </returns>
        protected override bool TryGetValue(string key, out object? value)
        {
            var found = base.TryGetValue(key, out value);
            if (!found)
            {
                var data = this.GetPluginData().Data;
                found = data.TryGetValue(key, out var stringValue);
                value = stringValue;
            }

            return found;
        }
    }
}
