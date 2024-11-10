// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.ExceptionHandling;

namespace Kephas.Entities;

/// <summary>
/// The result of a connection attempt.
/// </summary>
/// <param name="CanConnect">Indicates whether a connection is successful.</param>
/// <param name="ServerName">The server name.</param>
/// <param name="DatabaseName">The database name.</param>
/// <param name="Exception">Optional. An exception in case the a connection cannot be established.</param>
public record ConnectResult(bool CanConnect, string ServerName, string DatabaseName, ExceptionData? Exception = null);