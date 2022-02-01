// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextTenantExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

using Kephas.Application;

/// <summary>
/// Tenant related <see cref="IContext"/> extension methods.
/// </summary>
public static class ContextTenantExtensions
{
    /// <summary>
    /// Gets the tenant associated to the provided context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>The tenant associated to the provided context.</returns>
    public static string? Tenant(this IContext context)
    {
        context = context ?? throw new ArgumentNullException(nameof(context));

        return context.Injector.Resolve<IAppArgs>().Tenant();
    }
}