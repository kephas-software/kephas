// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;
using Kephas.Services.Reflection;

namespace Kephas;

/// <summary>
/// Collection of application service registrations.
/// </summary>
[ExcludeFromServices]
public class AppServiceCollection : List<IAppServiceInfo>, IAppServiceCollection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppServiceCollection"/> class.
    /// </summary>
    public AppServiceCollection()
    {
        this.Add<IAppServiceCollection>(this, b => b.IsExternallyOwned = true);
    }

    /// <summary>
    /// Replaces the service with the provided contract.
    /// </summary>
    /// <param name="appServiceInfo">The application service registration.</param>
    /// <returns>
    /// This <see cref="IAppServiceCollection"/>.
    /// </returns>
    public IAppServiceCollection Replace(IAppServiceInfo appServiceInfo)
    {
        var toDelete = this
            .Select((i, idx) => (appServiceInfo: i, index: idx))
            .Where(t => t.appServiceInfo.ContractType == appServiceInfo.ContractType)
            .Select(t => t.index)
            .OrderBy(i => i)
            .ToList();
        var delta = 0;
        foreach (var i in toDelete)
        {
            RemoveAt(i + (delta--));
        }

        Add(appServiceInfo);

        return this;
    }
}
