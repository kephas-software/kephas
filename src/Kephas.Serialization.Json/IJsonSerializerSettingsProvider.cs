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
    using Kephas.Services;

    using Newtonsoft.Json;

    /// <summary>
    /// Interface for JSON serializer settings provider.
    /// </summary>
    [SharedAppServiceContract]
    public interface IJsonSerializerSettingsProvider
    {
        /// <summary>
        /// Gets the JSON serializer settings.
        /// </summary>
        /// <returns>
        /// The JSON serializer settings.
        /// </returns>
        JsonSerializerSettings GetJsonSerializerSettings();
    }
}