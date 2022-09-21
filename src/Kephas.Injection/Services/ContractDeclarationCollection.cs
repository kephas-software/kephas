// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContractDeclarationCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

using System.Collections;

/// <summary>
/// Enumerates the contract declarations.
/// </summary>
public class ContractDeclarationCollection : IContractDeclarationCollection
{
    private readonly IEnumerable<ContractDeclaration> contracts;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContractDeclarationCollection"/> class.
    /// </summary>
    /// <param name="contracts">The contract declarations.</param>
    public ContractDeclarationCollection(IEnumerable<ContractDeclaration> contracts)
    {
        this.contracts = contracts ?? throw new ArgumentNullException(nameof(contracts));
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<ContractDeclaration> GetEnumerator() => this.contracts.GetEnumerator();

    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}