// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMongoClientSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.MongoDB;

using global::MongoDB.Driver;
using Kephas.Services;

/// <summary>
/// Provides the client settings based on the Mongo URL.
/// </summary>
[SingletonAppServiceContract]
public interface IMongoClientSettingsProvider
{
    /// <summary>
    /// Gets the client settings for the provided MongoDB URL.
    /// </summary>
    /// <param name="mongoUrl">The URL.</param>
    /// <returns>The MongoDB settings.</returns>
    MongoClientSettings GetClientSettings(MongoUrl mongoUrl);
}