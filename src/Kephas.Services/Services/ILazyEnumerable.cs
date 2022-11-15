// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILazyEnumerable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ILazyEnumerable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Kephas.Services.Resources;

/// <summary>
/// Interface for ordered lazy service collection.
/// </summary>
/// <typeparam name="TContract">Type of the service contract.</typeparam>
/// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
[AppServiceContract(AsOpenGeneric = true)]
public interface ILazyEnumerable<TContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata> : IEnumerable<Lazy<TContract, TMetadata>>
{
    /// <summary>
    /// Gets the services in the appropriate order.
    /// </summary>
    /// <param name="filter">Optional. Specifies a filter.</param>
    /// <returns>
    /// The ordered services.
    /// </returns>
    IEnumerable<TContract> SelectServices(Func<Lazy<TContract, TMetadata>, bool>? filter = null)
    {
        var factories = filter == null ? this : this.Where(filter);
        foreach (var factory in factories)
        {
            yield return factory.Value;
        }
    }

    /// <summary>
    /// Tries to get the service based on the provided criteria.
    /// </summary>
    /// <param name="criteria">The criteria function.</param>
    /// <returns>The requested service or <c>null</c>.</returns>
    TContract? TryGetService(Func<TMetadata, bool> criteria) =>
        this.SelectServices(l => criteria(l.Metadata)).FirstOrDefault();

    /// <summary>
    /// Gets the service based on the provided criteria.
    /// </summary>
    /// <param name="criteria">the criteria function.</param>
    /// <returns>If found, the requested service, otherwise an exception occurs.</returns>
    TContract GetService(Func<TMetadata, bool> criteria)
    {
        var service = this.TryGetService(criteria);
        return service ?? throw new ArgumentException(Strings.OrderedServiceFactoryCollection_service_with_requested_criteria_not_found, nameof(criteria));
    }
}
