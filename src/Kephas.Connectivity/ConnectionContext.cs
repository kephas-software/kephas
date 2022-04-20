// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Connectivity;

using Kephas.Injection;
using Kephas.Security.Authentication;
using Kephas.Services;

/// <summary>
/// A context for creating connections.
/// </summary>
/// <seealso cref="Context" />
/// <seealso cref="IConnectionContext" />
public class ConnectionContext : Context, IConnectionContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionContext"/> class.
    /// </summary>
    /// <param name="injector">The injector.</param>
    public ConnectionContext(IInjector injector)
        : base(injector)
    {
    }

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
    /// Gets or sets the exception during connection creation.
    /// </summary>
    public Exception? Exception { get; set; }
}
