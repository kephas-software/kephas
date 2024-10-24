// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExportFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract used to import parts that wish to dynamically create instances of other parts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Kephas.DependencyInjection;

/// <summary>
/// Contract used to import parts that wish to dynamically create instances of other parts.
/// </summary>
/// <typeparam name="T">The contract type of the created parts.</typeparam>
public interface IFactory<out T>
{
    /// <summary>
    /// Creates the exported value.
    /// </summary>
    /// <returns>
    /// A new instance of the exported value.
    /// </returns>
    public T Create();
}

/// <summary>
/// A contract for an export factory that provides metadata describing the created exports.
/// </summary>
/// <typeparam name="T">The contract type being created.</typeparam>
/// <typeparam name="TMetadata">The metadata required from the export.</typeparam>
public interface IFactory<out T, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] out TMetadata> : IFactory<T>
{
    /// <summary>
    /// Gets the metadata associated with the export.
    /// </summary>
    /// <value>
    /// The metadata associated with the export.
    /// </value>
    TMetadata Metadata { get; }
}