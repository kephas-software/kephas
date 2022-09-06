// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullEnabledEnumerable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behaviors;

using System.Collections;

/// <summary>
/// Service returning all services collection.
/// </summary>
/// <typeparam name="TContract">The type of the contract.</typeparam>
/// <seealso cref="IEnabledEnumerable{TContract}" />
[OverridePriority(Priority.Lowest)]
public class NullEnabledEnumerable<TContract> : IEnabledEnumerable<TContract>
{
    private readonly ICollection<TContract> services;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullEnabledEnumerable{TContract}"/> class.
    /// </summary>
    /// <param name="services">The services.</param>
    public NullEnabledEnumerable(ICollection<TContract> services)
    {
        this.services = services;
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<TContract> GetEnumerator() => this.services.GetEnumerator();

    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}