// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.MongoDB;

using Kephas.Configuration;
using Kephas.Dynamic;

/// <summary>
/// Provides MongoDB connection settings.
/// </summary>
public class MongoSettings : Expando, ISettings
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the database name.
    /// </summary>
    public string? DatabaseName { get; set; }
}