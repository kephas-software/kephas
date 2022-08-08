// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceSelector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection;

using Kephas.Services;

[OverridePriority(Priority.Low)]
public class ServiceSelector<TService, TMetadata> : IServiceSelector<TService, TMetadata>
    where TService : class
{
    private readonly IEnumerable<Lazy<TService, TMetadata>> services;

    public ServiceSelector(IEnumerable<Lazy<TService, TMetadata>> services)
    {
        this.services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public TService? TryGetService(Func<TMetadata, bool> selector)
    {
        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        var servicesQuery = typeof(IHasPriorityMetadata).IsAssignableFrom(typeof(TMetadata))
            ? services.OrderBy(s => ((IHasPriorityMetadata) s.Metadata!).Priority)
            : services;
        
        return servicesQuery.FirstOrDefault(s => selector(s.Metadata))?.Value;
    }
}
