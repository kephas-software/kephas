// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclareServiceAttributeBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.ServiceProviders;

namespace Kephas.DependencyInjection;

/// <summary>
/// Marks an interface or a class to be an application service, declared at assembly level.
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <typeparam name="TService">The service type.</typeparam>
public abstract class DeclareServiceAttributeBase<TService> : ServiceAttributeBase, IServiceInfo, IServiceInfoProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceAttributeBase"/> class.
    /// </summary>
    /// <param name="lifetime">The lifetime.</param>
    protected DeclareServiceAttributeBase(AppServiceLifetime lifetime)
        : base(lifetime)
    {
        ServiceType = typeof(TService);
    }

    /// <summary>
    /// Gets the type declaring the service type.
    /// </summary>
    /// <remarks>
    /// This is the type annotated with one of the: [<see cref="TransientServiceAttribute"/>], [<see cref="ScopedServiceAttribute"/>],
    /// or [<see cref="SingletonServiceAttribute"/>], and which declares the actual service type.
    /// Typically, this is a generic type used for collecting metadata, redirecting to the non-generic service type.
    /// </remarks>
    Type? IServiceInfo.ServiceDeclarationType => typeof(TService);

    /// <summary>
    /// Gets an enumeration of application service information objects and their contract declaration type.
    /// The contract declaration type is the type declaring the contract: if the <see cref="ServiceAttributeBase.ServiceType"/>
    /// is not provided, the contract declaration type is also the contract type.
    /// </summary>
    /// <returns>
    /// An enumeration of application service information objects and their contract declaration type.
    /// </returns>
    IEnumerable<ServiceDeclarationInfo> IServiceInfoProvider.GetServiceInfos()
    {
        yield return new ServiceDeclarationInfo(typeof(TService), this);
    }
}

/// <summary>
/// Marks an interface or a class to be an application service, declared at assembly level.
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <typeparam name="TService">The service type.</typeparam>
/// <typeparam name="TMetadata">The metadata type.</typeparam>
public abstract class DeclareServiceAttributeBase<TService, TMetadata> : DeclareServiceAttributeBase<TService>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceAttributeBase"/> class.
    /// </summary>
    /// <param name="lifetime">The lifetime.</param>
    protected DeclareServiceAttributeBase(AppServiceLifetime lifetime)
        : base(lifetime)
    {
        MetadataType = typeof(TMetadata);
    }
}