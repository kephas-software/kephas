// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMongoConventionsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.MongoDB.Conventions;

using global::MongoDB.Bson.Serialization.Conventions;

using Kephas.Services;

/// <summary>
/// Provides conventions for MongoDB client.
/// </summary>
/// <param name="Name">The registration name.</param>
/// <param name="ConventionPack">The conventions pack.</param>
/// <param name="Filter">Filter function for entity types.</param>
public record MongoConventions(string Name, ConventionPack ConventionPack, Func<Type, bool>? Filter);

/// <summary>
/// Service use for providing MongoDb conventions for initializing the driver.
/// </summary>
[AppServiceContract(AllowMultiple = true)]
public interface IMongoConventionsProvider
{
    /// <summary>
    /// Gets the conventions for initializing the driver.
    /// </summary>
    /// <returns>The conventions.</returns>
    MongoConventions GetConventions();
}