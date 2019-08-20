// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonSerializerConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IJsonSerializerConfigurator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text
{
    using Kephas.Services;

    /// <summary>
    /// Interface for JSON serializer configurator.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IJsonSerializerConfigurator
    {
        /// <summary>
        /// Configures the JSON serialization.
        /// </summary>
        /// <param name="overwrite">True to overwrite the configuration, false to preserve it (optional).</param>
        /// <returns>
        /// True if the configuration was changed, false otherwise.
        /// </returns>
        bool ConfigureJsonSerialization(bool overwrite = false);
    }
}