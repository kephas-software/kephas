// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesTenantExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas;

/// <summary>
/// Tenant related <see cref="IAmbientServices"/> extensions.
/// </summary>
public static class AmbientServicesTenantExtensions
{
    /// <summary>
    /// Adds multi-tenant support to <see cref="IAmbientServices"/>.
    /// </summary>
    /// <param name="ambientServices">The ambient services.</param>
    /// <typeparam name="T">The ambient services type.</typeparam>
    /// <returns>The provided <see cref="IAmbientServices"/>.</returns>
    public static T WithTenantSupport<T>(this T ambientServices)
        where T : IAmbientServices
    {
        // TODO - add tenant related probing folders for configuration and licenses.
        // TODO maybe a generic FolderManager service would be helpful, for example also
        // TODO to get from there the python probing folders. Other cases?
        return ambientServices;
    }
}