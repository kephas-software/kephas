// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Connectivity;

using Kephas.Security.Authentication;
using Kephas.Services;

/// <summary>
/// Provides a context while creating a connection.
/// </summary>
/// <seealso cref="IContext" />
public interface IConnectionContext : IContext
{
    /// <summary>
    /// Gets or sets the connection kind.
    /// </summary>
    /// <value>
    /// The kind.
    /// </value>
    public string? Kind { get; set; }

    /// <summary>
    /// Gets or sets the credentials.
    /// </summary>
    /// <value>
    /// The credentials.
    /// </value>
    public ICredentials? Credentials { get; set; }

    /// <summary>
    /// Gets or sets the host to connect to.
    /// </summary>
    /// <value>
    /// The host to connect to.
    /// </value>
    public Uri? Host { get; set; }

    /// <summary>
    /// Gets or sets the connection.
    /// </summary>
    public IConnection? Connection { get; set; }

    /// <summary>
    /// Gets or sets the connection exception.
    /// </summary>
    /// <value>
    /// The connection exception.
    /// </value>
    Exception? Exception { get; set; }
}
