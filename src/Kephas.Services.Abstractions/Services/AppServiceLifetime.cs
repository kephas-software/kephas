// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceLifetime.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

/// <summary>
/// Specifies the lifetime of a service.
/// </summary>
public enum AppServiceLifetime
{
    /// <summary>
    /// Specifies that a single instance of the service will be created.
    /// </summary>
    Singleton,
    /// <summary>
    /// Specifies that a new instance of the service will be created for each scope.
    /// </summary>
    /// <remarks>
    /// In ASP.NET Core applications a scope is created around each server request.
    /// </remarks>
    Scoped,
    /// <summary>
    /// Specifies that a new instance of the service will be created every time it is requested.
    /// </summary>
    Transient,
}
