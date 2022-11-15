// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullEnabledLazyServiceCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behaviors;

using System.Collections;

/// <summary>
/// Service returning all lazy services collection.
/// </summary>
/// <typeparam name="TContract">The type of the contract.</typeparam>
/// <typeparam name="TMetadata">The type of the metadata.</typeparam>
/// <seealso cref="IEnabledLazyServiceCollection{TContract, TMetadata}" />
[OverridePriority(Priority.Lowest)]
public class NullEnabledLazyServiceCollection<TContract, TMetadata> : IEnabledLazyServiceCollection<TContract, TMetadata>
{
    private readonly ICollection<Lazy<TContract, TMetadata>> services;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullEnabledLazyServiceCollection{TContract, TMetadata}"/> class.
    /// </summary>
    /// <param name="services">The services.</param>
    public NullEnabledLazyServiceCollection(ICollection<Lazy<TContract, TMetadata>> services)
    {
        this.services = services;
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<Lazy<TContract, TMetadata>> GetEnumerator()
        => this.services.GetEnumerator();

    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}