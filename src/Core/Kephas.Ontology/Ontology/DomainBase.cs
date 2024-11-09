// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Ontology;

/// <summary>
/// Base class for ontology elements implementing <see cref="IDomain"/>.
/// </summary>
public abstract class DomainBase : IDomain
{
    private readonly Lazy<IEnumerable<IDomain>> lazyDependencies;
    private readonly Lazy<DomainMetadata> lazyMetadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainBase"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    protected DomainBase(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        lazyMetadata = new Lazy<DomainMetadata>(() =>
            GetLazyDomains(serviceProvider)
                .FirstOrDefault(l => l.Metadata.ImplementationType == GetType())
                ?.Metadata
            ?? new DomainMetadata(null));

        lazyDependencies = new Lazy<IEnumerable<IDomain>>(() =>
            GetLazyDomains(serviceProvider)
                .SelectServices(m => lazyMetadata.Value.DomainDependencies?.Contains(m.ImplementationType) is true)
                .ToList());
    }

    private static ILazyEnumerable<IDomain, DomainMetadata> GetLazyDomains(IServiceProvider serviceProvider) =>
        (ILazyEnumerable<IDomain, DomainMetadata>)serviceProvider.GetRequiredService(typeof(ILazyEnumerable<IDomain, DomainMetadata>));

    /// <summary>
    /// Gets or sets the other domains that this domain is dependent upon.
    /// </summary>
    public IEnumerable<IDomain> Dependencies => lazyDependencies.Value;

    /// <summary>
    /// Gets the elements a domain consists of.
    /// </summary>
    public IEnumerable<IOntologyElement> Elements => [];

    /// <summary>
    /// Gets the name of the domain.
    /// </summary>
    public string Name => lazyMetadata.Value.ServiceKey ?? GetType().Name;
}