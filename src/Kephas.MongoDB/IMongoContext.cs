// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMongoContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.MongoDB;

using Kephas.Services;

/// <summary>
/// Provides a context for Mongo connections.
/// </summary>
public interface IMongoContext : IContext
{
    /// <summary>
    /// Gets the connection string.
    /// </summary>
    public string? ConnectionString { get; }

    /// <summary>
    /// Gets the name of the database.
    /// </summary>
    public string? DatabaseName { get; }
}