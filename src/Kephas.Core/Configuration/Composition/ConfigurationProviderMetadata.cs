// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationProviderMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the configuration provider metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services.Composition;

    /// <summary>
    /// A configuration provider metadata.
    /// </summary>
    public class ConfigurationProviderMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationProviderMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public ConfigurationProviderMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.SettingsType = (Type)metadata.TryGetValue(nameof(this.SettingsType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationProviderMetadata"/> class.
        /// </summary>
        /// <param name="settingsType">The type of the settings.</param>
        /// <param name="processingPriority">The processing priority (optional).</param>
        /// <param name="overridePriority">The override priority (optional).</param>
        /// <param name="optionalService">True to optional service (optional).</param>
        public ConfigurationProviderMetadata(Type settingsType, int processingPriority = 0, int overridePriority = 0, bool optionalService = false)
            : base(processingPriority, overridePriority, optionalService)
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