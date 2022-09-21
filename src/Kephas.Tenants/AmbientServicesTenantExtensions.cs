// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesTenantExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas;

using System.Diagnostics.CodeAnalysis;

using Kephas.Application;
using Kephas.IO;

/// <summary>
/// Tenant related <see cref="IAmbientServices"/> extensions.
/// </summary>
public static class AmbientServicesTenantExtensions
{
    /// <summary>
    /// Adds multi-tenant support to <see cref="IAmbientServices"/>.
    /// </summary>
    /// <param name="ambientServices">The ambient services.</param>
    /// <param name="appArgs">The application arguments.</param>
    /// <typeparam name="T">The ambient services type.</typeparam>
    /// <returns>The provided <see cref="IAmbientServices"/>.</returns>
    [return: NotNull]
    public static T UseTenantSupport<T>([DisallowNull] this T ambientServices, IAppArgs appArgs)
        where T : IAmbientServices
    {
        var tenant = appArgs.Tenant();
        if (string.IsNullOrEmpty(tenant))
        {
            return ambientServices;
        }

        ambientServices.Add<ILocationsManager>(new TenantFolderLocationsManager(tenant));
        return ambientServices;
    }
}