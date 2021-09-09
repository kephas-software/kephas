// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonSerializerSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Interface for JSON serializer settings provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json
{
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    using Newtonsoft.Json;

    /// <summary>
    /// Interface for JSON serializer settings provider.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IJsonSerializerSettingsProvider
    {
        /// <summary>
        /// Gets the JSON serializer settings.
        /// </summary>
        /// <returns>
        /// The JSON serializer settings.
        /// </returns>
        JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            this.ConfigureJsonSerializerSettings(settings);
            return settings;
        }

        /// <summary>
        /// Configures the provided json serializer settings.
        /// </summary>
        /// <param name="settings">The serializer settings to configure.</param>
        void ConfigureJsonSerializerSettings(JsonSerializerSettings settings);
    }
}