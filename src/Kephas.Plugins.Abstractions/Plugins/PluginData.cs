// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginData.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugin data class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Versioning;

    /// <summary>
    /// A plugin data.
    /// </summary>
    public sealed class PluginData
    {
        /// <summary>
        /// Gets the code for the mismatched identity.
        /// </summary>
        internal const int MismatchedIdentityCode = 10;

        private const int MissingPartsInvalidCode = 1;
        private const int ParseStateInvalidCode = 2;
        private const int ParseKindInvalidCode = 3;
        private const int ParseChecksumInvalidCode = 4;
        private const int ChecksumInvalidCode = 100;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginData"/> class.
        /// </summary>
        /// <param name="identity">The plugin identity.</param>
        /// <param name="state">The plugin state.</param>
        /// <param name="kind">Optional. The plugin kind.</param>
        /// <param name="data">Optional. The additional data associated with the plugin.</param>
        public PluginData(AppIdentity identity, PluginState state, PluginKind kind = PluginKind.Embedded, IDictionary<string, string?>? data = null)
        {
            Requires.NotNull(identity, nameof(identity));

            this.Identity = identity;
            this.State = state;
            this.Kind = kind;
            this.Data = data ?? new Dictionary<string, string?>();
        }

        /// <summary>
        /// Gets the identifier of the plugin.
        /// </summary>
        /// <value>
        /// The identifier of the plugin.
        /// </value>
        public AppIdentity Identity { get; private set; }

        /// <summary>
        /// Gets the plugin state.
        /// </summary>
        /// <value>
        /// The plugin state.
        /// </value>
        public PluginState State { get; private set; }

        /// <summary>
        /// Gets the plugin kind.
        /// </summary>
        /// <value>
        /// The plugin kind.
        /// </value>
        public PluginKind Kind { get; private set; }

        /// <summary>
        /// Gets additional data associated with the license.
        /// </summary>
        /// <value>
        /// The additional data associated with the license.
        /// </value>
        public IDictionary<string, string?> Data { get; }

        /// <summary>
        /// Gets or sets the version towards which the plugin is being updated.
        /// </summary>
        internal string? UpdatingToVersion
        {
            get => this.Data.TryGetValue(nameof(this.UpdatingToVersion));
            set => this.Data[nameof(this.UpdatingToVersion)] = value;
        }

        /// <summary>
        /// Parses the value and returns valid plugin data.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="throwOnInvalid">Optional. Indicates whether to throw on invalid plugin data or to return a corrupt-marked data.</param>
        /// <returns>
        /// A PluginData.
        /// </returns>
        public static PluginData Parse(string value, bool throwOnInvalid = true)
        {
            value = value ?? throw new ArgumentNullException(nameof(value));

            var splits = value.Split('\n');
            var appId = AppIdentity.Parse(splits[0]);

            if (splits.Length < 3)
            {
                return throwOnInvalid
                    ? throw new InvalidPluginDataException(
                        $"The plugin data for {appId} is corrupt, probably was manually changed ({MissingPartsInvalidCode}).",
                        MissingPartsInvalidCode)
                    : new PluginData(appId, PluginState.Corrupt);
            }

            if (!Enum.TryParse<PluginState>(splits[1], out var state))
            {
                return throwOnInvalid
                    ? throw new InvalidPluginDataException(
                        $"The plugin data for {appId} is corrupt, probably was manually changed ({ParseStateInvalidCode}).",
                        ParseStateInvalidCode)
                    : new PluginData(appId, PluginState.Corrupt);
            }

            if (!Enum.TryParse<PluginKind>(splits[2], out var kind))
            {
                return throwOnInvalid
                    ? throw new InvalidPluginDataException($"The plugin data for {appId} is corrupt, probably was manually changed ({ParseKindInvalidCode}).", ParseKindInvalidCode)
                    : new PluginData(appId, PluginState.Corrupt);
            }

            var data = splits.Length > 3 ? DataParse(splits[3..^1]) : null;
            if (!int.TryParse(splits[^1], out var checksum))
            {
                return throwOnInvalid
                    ? throw new InvalidPluginDataException($"The plugin data for {appId} is corrupt, probably was manually changed ({ParseChecksumInvalidCode}).", ParseChecksumInvalidCode)
                    : new PluginData(appId, PluginState.Corrupt, kind, data);
            }

            var pluginData = new PluginData(appId, state, kind, data);
            return pluginData.Validate(checksum, throwOnInvalid)
                ? pluginData
                : new PluginData(appId, PluginState.Corrupt, kind, data);
        }

        /// <summary>
        /// Changes the identity of this instance with the provided identity.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <returns>
        /// This <see cref="PluginData"/>.
        /// </returns>
        public PluginData ChangeIdentity(AppIdentity pluginIdentity)
        {
            Requires.NotNull(pluginIdentity, nameof(pluginIdentity));

            if (!this.Identity.Id.Equals(pluginIdentity.Id, StringComparison.CurrentCultureIgnoreCase)
                || !(this.Identity.Version?.Equals(pluginIdentity.Version)
                    ?? true))
            {
                throw new InvalidPluginDataException($"Cannot change identity from {this.Identity} to {pluginIdentity}, only casing differences are accepted.", MismatchedIdentityCode);
            }

            this.Identity = pluginIdentity;
            return this;
        }

        /// <summary>
        /// Changes the state of this instance with the provided changed state.
        /// </summary>
        /// <param name="state">The plugin state.</param>
        /// <returns>
        /// This <see cref="PluginData"/>.
        /// </returns>
        public PluginData ChangeState(PluginState state)
        {
            this.State = state;
            return this;
        }

        /// <summary>
        /// Changes the kind of this instance with the provided changed state.
        /// </summary>
        /// <param name="kind">The plugin kind.</param>
        /// <returns>
        /// This <see cref="PluginData"/>.
        /// </returns>
        public PluginData ChangeKind(PluginKind kind)
        {
            this.Kind = kind;
            return this;
        }

        /// <summary>
        /// Changes the data value of this instance with the provided new value.
        /// </summary>
        /// <param name="key">The data entry key.</param>
        /// <param name="value">The data entry value.</param>
        /// <returns>
        /// This <see cref="PluginData"/>.
        /// </returns>
        public PluginData ChangeData(string key, string? value)
        {
            this.Data[key] = value;
            return this;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Identity}\n{this.State}\n{this.Kind}\n{DataToString(this.Data)}\n{this.GetChecksum()}";
        }

        /// <summary>
        /// Gets or sets the version towards which the plugin is being updated.
        /// </summary>
        /// <param name="version">The version to be updated to.</param>
        /// <returns>The plugin data.</returns>
        internal PluginData ChangeUpdatingToVersion(SemanticVersion? version)
            => this.ChangeData(nameof(this.UpdatingToVersion), version?.ToString());

        private static IDictionary<string, string?> DataParse(IEnumerable<string> values)
        {
            var data = new Dictionary<string, string?>();
            foreach (var value in values)
            {
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                var pos = value.IndexOf(':');
                if (pos >= 0)
                {
                    data[value.Substring(0, pos)] = value.Substring(pos + 1);
                }
                else
                {
                    data[value] = null;
                }
            }

            return data;
        }

        private static string DataToString(IDictionary<string, string?>? data)
        {
            if (data == null || data.Count == 0)
            {
                return string.Empty;
            }

            return string.Join("\n", data.Select(kv => $"{kv.Key}:{kv.Value}"));
        }

        private bool Validate(int checksum, bool throwOnInvalid)
        {
            if (this.GetChecksum() == checksum)
            {
                return true;
            }

            return throwOnInvalid
                ? throw new InvalidPluginDataException($"The plugin data for {this.Identity} is corrupt, probably was manually changed ({ChecksumInvalidCode}).", ChecksumInvalidCode)
                : false;
        }

        private int GetChecksum()
        {
            var hashCodeGenerator = new HashCodeGenerator()
                .CombineStable(this.Identity.ToString())
                .Combine(this.State)
                .Combine(this.Kind)
                .CombineStable(DataToString(this.Data));
            return hashCodeGenerator.GeneratedHash;
        }
    }
}
