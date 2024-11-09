// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Connectivity;

using Kephas.Security.Authentication;
using Kephas.Services;

/// <summary>
/// Central service for creating connections.
/// </summary>
[SingletonAppServiceContract]
public interface IConnectionProvider
{
    /// <summary>
    /// Creates the connection configured through the connection options.
    /// </summary>
    /// <param name="host">The host URI.</param>
    /// <param name="credentials">Optional. The credentials. Although typically required, not all connections need credentials.</param>
    /// <param name="kind">Optional. The connection kind. If not provided, the host scheme is used.</param>
    /// <param name="options">Optional. Other options for connection configuration.</param>
    /// <returns>
    /// The newly created connection.
    /// </returns>
    IConnection CreateConnection(Uri host, ICredentials? credentials = null, string? kind = null, Action<IConnectionContext>? options = null);

    /// <summary>
    /// Creates the connection configured through the connection options.
    /// </summary>
    /// <param name="host">The host URI as string.</param>
    /// <param name="credentials">Optional. The credentials. Although typically required, not all connections need credentials.</param>
    /// <param name="kind">Optional. The connection kind. If not provided, the host scheme is used.</param>
    /// <param name="options">Optional. Other options for connection configuration.</param>
    /// <returns>
    /// The newly created connection.
    /// </returns>
    IConnection CreateConnection(string host, ICredentials? credentials = null, string? kind = null, Action<IConnectionContext>? options = null)
        => this.CreateConnection(new Uri(host ?? throw new ArgumentNullException(nameof(host))), credentials, kind, options);
}