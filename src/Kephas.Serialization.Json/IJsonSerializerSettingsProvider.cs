// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonSerializerSettingsProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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