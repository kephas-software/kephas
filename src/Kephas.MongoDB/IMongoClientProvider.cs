// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMongoClientProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.MongoDB;

using global::MongoDB.Driver;
using Kephas.Services;

/// <summary>
/// Provides access to the IMongoClient.
/// </summary>
[SingletonAppServiceContract]
public interface IMongoClientProvider
{
    /// <summary>
    /// Gets the configured <see cref="IMongoClient"/> instance.
    /// </summary>
    /// <param name="context">The mongo context.</param>
    /// <returns>The configured <see cref="IMongoClient"/> instance.</returns>
    IMongoClient GetMongoClient(IMongoContext? context = null);
}