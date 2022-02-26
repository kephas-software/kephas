// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Connectivity;

using Kephas.Services;

/// <summary>
/// Factory for connections of a certain kind.
/// </summary>
[SingletonAppServiceContract(AllowMultiple = true)]
public interface IConnectionFactory
{
    /// <summary>
    /// Creates the connection configured through the connection context.
    /// </summary>
    /// <param name="context">The connection creation context.</param>
    /// <returns>The newly created connection.</returns>
    IConnection CreateConnection(IConnectionContext context);
}
