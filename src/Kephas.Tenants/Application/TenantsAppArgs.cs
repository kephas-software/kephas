// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantsAppArgs.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application;

/// <summary>
/// Additions to <see cref="IAppArgs"/> for tenant support.
/// </summary>
public static class TenantsAppArgs
{
    /// <summary>
    /// The 'tenant' application argument name.
    /// </summary>
    public static readonly string TenantAppArgName = "tenant";

    /// <summary>
    /// Gets the tenant identifier.
    /// </summary>
    /// <param name="appArgs">The application arguments.</param>
    /// <returns>The tenant identifier.</returns>
    public static string? Tenant(this IAppArgs appArgs) => appArgs[TenantAppArgName] as string;
}