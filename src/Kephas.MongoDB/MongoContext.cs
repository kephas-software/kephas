// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.MongoDB;

using Kephas.Services;
using Kephas.Services;

/// <summary>
/// The implementation of the <see cref="IMongoContext"/>.
/// </summary>
public class MongoContext : Context, IMongoContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoContext"/> class.
    /// </summary>
    /// <param name="serviceProvider">The injector.</param>
    public MongoContext(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the name of the database.
    /// </summary>
    public string? DatabaseName { get; set; }
}