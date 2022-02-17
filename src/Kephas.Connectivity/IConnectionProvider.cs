// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Connectivity;

/// <summary>
/// Central service for creating connections.
/// </summary>
[SingletonAppServiceContract(AllowMultiple = true)]
public interface IConnectionProvider
{
    /// <summary>
    /// Creates the connection configured through the connection options.
    /// </summary>
    /// <param name="options">The options for connection configuration.</param>
    /// <returns>
    /// The newly created connection.
    /// </returns>
    IConnection CreateConnection(Action<IConnectionContext> options);
}