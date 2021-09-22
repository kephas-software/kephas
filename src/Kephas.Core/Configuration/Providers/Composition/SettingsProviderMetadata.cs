// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationProviderMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the configuration provider metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration.Providers.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services;

    /// <summary>
    /// A settings provider metadata.
    /// </summary>
    public class SettingsProviderMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsProviderMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public SettingsProviderMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.SettingsType = (Type)metadata.TryGetValue(nameof(this.SettingsType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsProviderMetadata"/> class.
        /// </summary>
        /// <param name="settingsType">The type of the settings.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        public SettingsProviderMetadata(Type settingsType, Priority processingPriority = 0, Priority overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            this.SettingsType = settingsType;
        }

        /// <summary>
        /// Gets the type of the settings.
        /// </summary>
        /// <value>
        /// The type of the settings.
        /// </value>
        public Type SettingsType { get; }
    }
}