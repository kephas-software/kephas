// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas;

using System.Diagnostics.CodeAnalysis;

using Kephas.Application;
using Kephas.IO;
using Kephas.Services.Builder;

/// <summary>
/// Tenant related <see cref="IAppServiceCollection"/> extensions.
/// </summary>
public static class TenantServicesExtensions
{
    /// <summary>
    /// Adds multi-tenant support to <see cref="IAppServiceCollectionBuilder"/>.
    /// </summary>
    /// <param name="servicesBuilder">The services builder.</param>
    /// <param name="appArgs">The application arguments.</param>
    /// <typeparam name="T">The app services type.</typeparam>
    /// <returns>The provided <paramref name="servicesBuilder"/>.</returns>
    [return: NotNull]
    public static IAppServiceCollectionBuilder WithTenantSupport(this IAppServiceCollectionBuilder servicesBuilder, IAppArgs appArgs)
    {
        var tenant = appArgs.Tenant();
        if (string.IsNullOrEmpty(tenant))
        {
            return servicesBuilder;
        }

        servicesBuilder.AppServices.Add<ILocationsManager>(new TenantFolderLocationsManager(tenant));
        return servicesBuilder;
    }
}