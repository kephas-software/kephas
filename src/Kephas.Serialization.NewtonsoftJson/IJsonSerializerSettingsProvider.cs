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
#if NETSTANDARD2_1
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
#endif

        /// <summary>
        /// Configures the provided json serializer settings.
        /// </summary>
        /// <param name="settings">The serializer settings to configure.</param>
        void ConfigureJsonSerializerSettings(JsonSerializerSettings settings);
    }

#if NETSTANDARD2_1
#else
    /// <summary>
    /// Extension methods for <see cref="IJsonSerializerSettingsProvider"/>.
    /// </summary>
    public static class JsonSerializerSettingsExtensions
    {
        /// <summary>
        /// Gets the JSON serializer settings.
        /// </summary>
        /// <param name="settingsProvider">The JSON settings provider.</param>
        /// <returns>
        /// The JSON serializer settings.
        /// </returns>
        public static JsonSerializerSettings GetJsonSerializerSettings(
            this IJsonSerializerSettingsProvider settingsProvider)
        {
            Requires.NotNull(settingsProvider, nameof(settingsProvider));

            var settings = new JsonSerializerSettings();
            settingsProvider.ConfigureJsonSerializerSettings(settings);
            return settings;
        }
    }
#endif
}