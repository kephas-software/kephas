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
/// Base service used to retrieve service instances on a factory basis.
/// </summary>
public interface IFactoryBase
{
    /// <summary>
    /// Gets an instance retrieved from the dependency injection container.
    /// </summary>
    public object GetService();
}

/// <summary>
/// Service used to retrieve service instances on a factory basis, including metadata.
/// </summary>
/// <typeparam name="T">The contract type being created.</typeparam>
/// <typeparam name="TMetadata">The metadata required from the export.</typeparam>
public interface IFactoryBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] out TMetadata> : IFactoryBase
{
    /// <summary>
    /// Gets the metadata associated with the service.
    /// </summary>
    /// <value>
    /// The metadata associated with the service.
    /// </value>
    TMetadata Metadata { get; }
}

/// <summary>
/// Service used to retrieve service instances on a factory basis.
/// </summary>
/// <typeparam name="T">The service type of the retrieved instances.</typeparam>
public interface IFactory<out T> : IFactoryBase
    where T : notnull
{
    /// <summary>
    /// Gets an instance retrieved from the dependency injection container.
    /// </summary>
    public new T GetService();

    /// <summary>
    /// Gets an instance retrieved from the dependency injection container.
    /// </summary>
    object IFactoryBase.GetService() => GetService();
}

/// <summary>
/// Service used to retrieve service instances on a factory basis, including metadata.
/// </summary>
/// <typeparam name="T">The contract type being created.</typeparam>
/// <typeparam name="TMetadata">The metadata required from the export.</typeparam>
public interface IFactory<out T, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] out TMetadata> : IFactory<T>, IFactoryBase<TMetadata>
    where T : notnull
{
}