// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContractDeclarationCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;

namespace Kephas.DependencyInjection.ServiceProviders;

/// <summary>
/// Enumerates the contract declarations.
/// </summary>
internal class ServiceDeclarationInfoCollection(IList<ServiceDeclarationInfo> declarations) : IServiceDeclarationInfoCollection
{
    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<ServiceDeclarationInfo> GetEnumerator() => declarations.GetEnumerator();

    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}