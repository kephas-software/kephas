// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Reflection;

namespace Kephas;

/// <summary>
/// Collection of application service registrations.
/// </summary>
[ExcludeFromDependencyInjection]
public class AppServiceCollection : List<IServiceInfo>, IAppServiceCollection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppServiceCollection"/> class.
    /// </summary>
    public AppServiceCollection()
    {
        this.Add<IAppServiceCollection>(this);
    }

    /// <summary>
    /// Replaces the service with the provided contract.
    /// </summary>
    /// <param name="serviceInfo">The application service registration.</param>
    /// <returns>
    /// This <see cref="IAppServiceCollection"/>.
    /// </returns>
    public IAppServiceCollection Replace(IServiceInfo serviceInfo)
    {
        var toDelete = this
            .Select((i, idx) => (appServiceInfo: i, index: idx))
            .Where(t => t.appServiceInfo.ServiceType == serviceInfo.ServiceType)
            .Select(t => t.index)
            .OrderBy(i => i)
            .ToList();
        var delta = 0;
        foreach (var i in toDelete)
        {
            RemoveAt(i + (delta--));
        }

        Add(serviceInfo);

        return this;
    }
}
