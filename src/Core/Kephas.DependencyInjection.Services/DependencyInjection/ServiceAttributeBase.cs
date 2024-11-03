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
/// Enumerates the generic service kind.
/// </summary>
public enum GenericRegistration
{
    /// <summary>
    /// The default for the provided service type is considered: None for non-generic types, Open for open generic types, Constructed for the rest.
    /// </summary>
    Default,
    
    /// <summary>
    /// Register open generic services.
    /// </summary>
    AsOpen,
    
    /// <summary>
    /// Register closed generic services.
    /// </summary>
    AsClosed,
}

/// <summary>
/// Marks an interface or a class to be a transient application service contract.
/// </summary>
public abstract class ServiceAttributeBase : Attribute, IServiceInfo
{
    private readonly GenericRegistration genericRegistration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceAttributeBase"/> class.
    /// </summary>
    /// <param name="lifetime">The lifetime.</param>
    /// <param name="genericRegistration">The generic service kind.</param>
    protected ServiceAttributeBase(AppServiceLifetime lifetime, GenericRegistration genericRegistration)
    {
        Lifetime = lifetime;
        this.genericRegistration = genericRegistration;
    }

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
    public bool? AsOpenGeneric => genericRegistration.ToNullableBoolean(ServiceType);

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
        var openGeneric = 
            AsOpenGeneric is null
                ? string.Empty :
                AsOpenGeneric is true
                    ? ", open generic"
                    : ", closed generic";
        var metadata = MetadataType is null ? string.Empty : $", {MetadataType.Name}";
        return $"{ServiceType}{openGeneric}, {Lifetime}{metadata}";
    }
}
