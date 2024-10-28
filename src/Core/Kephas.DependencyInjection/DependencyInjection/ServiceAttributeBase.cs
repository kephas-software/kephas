// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceContractAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Marks an interface to be an application service.
//   Application services are automatically identified by the composition
//   and added to the container.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Reflection;

namespace Kephas.DependencyInjection;

/// <summary>
/// Marks an interface or a class to be a transient application service contract.
/// </summary>
public abstract class ServiceAttributeBase : Attribute, IServiceInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceAttributeBase"/> class.
    /// </summary>
    /// <param name="lifetime">The lifetime.</param>
    protected ServiceAttributeBase(AppServiceLifetime lifetime) => Lifetime = lifetime;

    /// <summary>
    /// Gets the application service lifetime.
    /// </summary>
    /// <value>
    /// The application service lifetime.
    /// </value>
    public AppServiceLifetime Lifetime { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the contract should be exported as an open generic.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the contract should be exported as an open generic; otherwise, <c>false</c>.
    /// </value>
    public bool AsOpenGeneric { get; set; }

    /// <summary>
    /// Gets or sets the contract type of the export.
    /// </summary>
    /// <value>
    /// The contract type of the export.
    /// </value>
    public Type? ServiceType { get; set; }

    /// <summary>
    /// Gets or sets the supported metadata type.
    /// </summary>
    /// <value>
    /// The supported metadata type.
    /// </value>
    public Type? MetadataType { get; set; }

    /// <summary>
    /// Gets the instancing strategy: factory, type, or instance.
    /// </summary>
    public object? InstancingStrategy { get; protected set; } = null;

    /// <summary>
    /// Throws an exception indicating that metadata cannot be added to attributes.
    /// </summary>
    /// <param name="name">The metadata name.</param>
    /// <param name="value">The metadata value.</param>
    /// <returns>This <see cref="IServiceInfo"/>.</returns>
    IServiceInfo IServiceInfo.AddMetadata(string name, object? value) =>
        throw new NotSupportedException("Cannot add metadata to attribute instances.");

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    public override string ToString()
    {
        var openGeneric = AsOpenGeneric ? ", open generic" : string.Empty;

        return $"{ServiceType}{openGeneric}, {Lifetime}";
    }
}
