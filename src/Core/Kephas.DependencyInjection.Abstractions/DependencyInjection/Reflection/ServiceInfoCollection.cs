// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceInfoCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection.Reflection;

/// <summary>
/// The collection of <see cref="IServiceInfo"/>.
/// </summary>
public class ServiceInfoCollection : List<IServiceInfo>, IServiceInfoCollection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceInfoCollection"/>.
    /// </summary>
    /// <param name="services">The services.</param>
    public ServiceInfoCollection(IEnumerable<IServiceInfo> services) : base(services)
    {
        Add(new ServiceInfo(typeof(IServiceInfoCollection), this));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceInfoCollection"/>.
    /// </summary>
    public ServiceInfoCollection()
    {
        Add(new ServiceInfo(typeof(IServiceInfoCollection), this));
    }
}