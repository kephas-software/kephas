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

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A plugin data.
    /// </summary>
    public class PluginData
    {
        private const int MissingPartsInvalidCode = 1;
        private const int ParseStateInvalidCode = 2;
        private const int ParseChecksumInvalidCode = 3;
        private const int CheksumInvalidCode = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginData"/> class.
        /// </summary>
        /// <param name="identity">The plugin identity.</param>
        /// <param name="state">The plugin state.</param>
        public PluginData(AppIdentity identity, PluginState state)
        {
            Requires.NotNull(identity, nameof(identity));

            this.Identity = identity;
            this.State = state;
        }

        /// <summary>
        /// Gets the identifier of the plugin.
        /// </summary>
        /// <value>
        /// The identifier of the plugin.
        /// </value>
        public AppIdentity Identity { get; }

        /// <summary>
        /// Gets the plugin state.
        /// </summary>
        /// <value>
        /// The plugin state.
        /// </value>
        public PluginState State { get; }

        /// <summary>
        /// Parses the value and returns valid plugin data.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A PluginData.
        /// </returns>
        public static PluginData Parse(string value)
        {
            Requires.NotNull(value, nameof(value));

            var splits = value.Split(',');
            var appId = AppIdentity.Parse(splits[0]);
            if (splits.Length < 3)
            {
                throw new InvalidPluginDataException($"The plugin data for {appId} is corrupt, probably was manually changed ({MissingPartsInvalidCode}).");
            }

            if (!Enum.TryParse<PluginState>(splits[1], out var state))
            {
                throw new InvalidPluginDataException($"The plugin data for {appId} is corrupt, probably was manually changed ({ParseStateInvalidCode}).");
            }

            if (!int.TryParse(splits[2], out var checksum))
            {
                throw new InvalidPluginDataException($"The plugin data for {appId} is corrupt, probably was manually changed ({ParseChecksumInvalidCode}).");
            }

            var pluginData = new PluginData(appId, state);
            pluginData.Validate(checksum);

            return pluginData;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Identity},{this.State},{this.GetChecksum()}";
        }

        private void Validate(int checksum)
        {
            if (this.GetChecksum() == checksum)
            {
                return;
            }

            throw new InvalidPluginDataException($"The plugin data for {this.Identity} is corrupt, probably was manually changed ({CheksumInvalidCode}).");
        }

        private int GetChecksum()
        {
            var str = $"{this.Identity},{this.State}";
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                    {
                        break;
                    }

                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }
}
