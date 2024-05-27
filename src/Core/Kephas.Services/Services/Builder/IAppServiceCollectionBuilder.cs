// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceCollectionBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppServiceCollectionBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;

namespace Kephas.Services.Builder;

/// <summary>
/// Contract interface for <see cref="IAppServiceCollection"/> builders.
/// </summary>
public interface IAppServiceCollectionBuilder
{
    /// <summary>
    /// Adds the <see cref="IAppServiceInfoProvider"/>s.
    /// </summary>
    /// <param name="providers">An array of providers.</param>
    /// <returns>This builder.</returns>
    IAppServiceCollectionBuilder AddProviders(params IAppServiceInfoProvider[] providers);

    /// <summary>
    /// Adds the <see cref="Assembly"/>s.
    /// </summary>
    /// <param name="assemblies">An array of assemblies.</param>
    /// <returns>This builder.</returns>
    IAppServiceCollectionBuilder AddAssemblies(params Assembly[] assemblies);

    /// <summary>
    /// Adds the <see cref="Type"/>s.
    /// </summary>
    /// <param name="types">An array of types.</param>
    /// <returns>This builder.</returns>
    IAppServiceCollectionBuilder AddTypes(params Type[] types);

    /// <summary>
    /// Adds the application services from the <see cref="IAppServiceInfoProvider"/>s identified in the assemblies.
    /// </summary>
    /// <returns>The provided app services.</returns>
    IAppServiceCollection Build();
}
