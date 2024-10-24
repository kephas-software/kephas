// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Reflection;

namespace Kephas;

/// <summary>
/// Provides a registration container for application services.
/// </summary>
public interface IAppServiceCollection : IList<IServiceInfo>
{
    private static readonly List<Action<IAppServiceCollection>> Collectors = new ();

    /// <summary>
    /// Gets a value indicating whether the service with the provided contract is registered.
    /// </summary>
    /// <param name="contractType">Type of the service contract.</param>
    /// <returns>
    /// <c>true</c> if the service is registered, <c>false</c> if not.
    /// </returns>
    public bool Contains(Type contractType)
        => this.Any(r => r.ServiceType == contractType);

    /// <summary>
    /// Replaces the service with the provided contract.
    /// </summary>
    /// <param name="serviceInfo">The application service registration.</param>
    /// <returns>
    /// This <see cref="IAppServiceCollection"/>.
    /// </returns>
    public IAppServiceCollection Replace(IServiceInfo serviceInfo);

    /// <summary>
    /// Adds an collector for <see cref="IAppServiceCollection"/>.
    /// </summary>
    /// <param name="collect">The collect callback function.</param>
    public static void AddAppServicesCollector(Action<IAppServiceCollection> collect)
    {
        lock (Collectors)
        {
            Collectors.Add(collect ?? throw new ArgumentNullException(nameof(collect)));
        }
    }

    /// <summary>
    /// Initializes the app services with the registered collectors.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    public static void RegisterCollectedAppServices(IAppServiceCollection appServices)
    {
        lock (Collectors)
        {
            foreach (var collect in Collectors)
            {
                collect(appServices);
            }
        }
    }
}
