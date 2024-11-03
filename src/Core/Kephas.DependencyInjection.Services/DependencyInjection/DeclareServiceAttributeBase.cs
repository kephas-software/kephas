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
public abstract class DeclareServiceAttributeBase : ServiceAttributeBase, IServiceInfo, IServiceInfoProvider
{
    private readonly Type implementationType;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeclareServiceAttributeBase"/> class.
    /// </summary>
    /// <param name="serviceType">The service type.</param>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="lifetime">The lifetime.</param>
    /// <param name="genericRegistration">The generic service kind.</param>
    protected DeclareServiceAttributeBase(Type serviceType, Type implementationType, AppServiceLifetime lifetime, GenericRegistration genericRegistration = GenericRegistration.Default)
        : base(lifetime, implementationType?.IsGenericTypeDefinition is true ? genericRegistration : GenericRegistration.AsClosed)
    {
        ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        InstancingStrategy = this.implementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));

        if (!ServiceType.IsGenericTypeDefinition && !ServiceType.IsAssignableFrom(implementationType))
        {
            throw new InvalidOperationException($"{ServiceType} must be assignable to {implementationType}.");
        }
    }

    /// <summary>
    /// Gets the type declaring the service type.
    /// </summary>
    /// <remarks>
    /// This is the type annotated with one of the: [<see cref="TransientServiceAttribute"/>], [<see cref="ScopedServiceAttribute"/>],
    /// or [<see cref="SingletonServiceAttribute"/>], and which declares the actual service type.
    /// Typically, this is a generic type used for collecting metadata, redirecting to the non-generic service type.
    /// </remarks>
    Type? IServiceInfo.ServiceDeclarationType => ServiceType;

    /// <summary>
    /// Gets an enumeration of <see cref="ServiceDeclaration"/>.
    /// </summary>
    /// <returns>
    /// An enumeration of <see cref="ServiceDeclaration"/>.
    /// </returns>
    IEnumerable<ServiceDeclaration>? IServiceInfoProvider.GetServiceDeclarations()
    {
        yield return new ServiceDeclaration(ServiceType!, MetadataType);
    }

    /// <summary>
    /// Gets an enumeration of <see cref="ServiceDeclarationInfo"/>.
    /// The service declaration type is the type declaring the service: if the <see cref="ServiceAttributeBase.ServiceType"/>
    /// is not provided, the service declaration type is also the service type.
    /// </summary>
    /// <returns>
    /// An enumeration of <see cref="ServiceDeclarationInfo"/>.
    /// </returns>
    IEnumerable<ServiceDeclarationInfo> IServiceInfoProvider.GetServiceDeclarationInfos()
    {
        yield return new ServiceDeclarationInfo(ServiceType!, new ServiceInfo(ServiceType!, Lifetime, implementationType.IsGenericTypeDefinition) { MetadataType = MetadataType });
    }

    /// <summary>
    /// Gets an enumeration of <see cref="ServiceImplementationDeclaration"/>.
    /// </summary>
    /// <param name="eligibleImplementationTypes">The eligible implementation types.</param>
    /// <returns>
    /// An enumeration of <see cref="ServiceImplementationDeclaration"/>.
    /// </returns>
    IEnumerable<ServiceImplementationDeclaration> IServiceInfoProvider.GetImplementationDeclarations(
        IEnumerable<Type> eligibleImplementationTypes)
    {
        yield return new ServiceImplementationDeclaration(implementationType, ServiceType!);
    }
}

/// <summary>
/// Marks an interface or a class to be an application service, declared at assembly level.
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <remarks>
/// This kind of declaration does not allow registrations as open generic.
/// </remarks>
/// <typeparam name="TService">The service type.</typeparam>
/// <typeparam name="TImplementation">The implementation type.</typeparam>
public abstract class DeclareServiceAttributeBase<TService, TImplementation> : ServiceAttributeBase, IServiceInfo, IServiceInfoProvider
    where TImplementation : TService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeclareServiceAttributeBase{TService,TImplementation}"/> class.
    /// </summary>
    /// <param name="lifetime">The lifetime.</param>
    protected DeclareServiceAttributeBase(AppServiceLifetime lifetime)
        : base(lifetime, GenericRegistration.AsClosed)
    {
        ServiceType = typeof(TService);
        InstancingStrategy = typeof(TImplementation);
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
    /// Gets an enumeration of <see cref="ServiceDeclaration"/>.
    /// </summary>
    /// <returns>
    /// An enumeration of <see cref="ServiceDeclaration"/>.
    /// </returns>
    IEnumerable<ServiceDeclaration>? IServiceInfoProvider.GetServiceDeclarations()
    {
        yield return new ServiceDeclaration(ServiceType!, MetadataType);
    }

    /// <summary>
    /// Gets an enumeration of <see cref="ServiceDeclarationInfo"/>.
    /// The service declaration type is the type declaring the service: if the <see cref="ServiceAttributeBase.ServiceType"/>
    /// is not provided, the service declaration type is also the service type.
    /// </summary>
    /// <returns>
    /// An enumeration of <see cref="ServiceDeclarationInfo"/>.
    /// </returns>
    IEnumerable<ServiceDeclarationInfo> IServiceInfoProvider.GetServiceDeclarationInfos()
    {
        yield return new ServiceDeclarationInfo(ServiceType!, new ServiceInfo(ServiceType!, Lifetime, typeof(TImplementation).IsGenericTypeDefinition) { MetadataType = MetadataType });
    }

    /// <summary>
    /// Gets an enumeration of <see cref="ServiceImplementationDeclaration"/>.
    /// </summary>
    /// <param name="eligibleImplementationTypes">The eligible implementation types.</param>
    /// <returns>
    /// An enumeration of <see cref="ServiceImplementationDeclaration"/>.
    /// </returns>
    IEnumerable<ServiceImplementationDeclaration> IServiceInfoProvider.GetImplementationDeclarations(IEnumerable<Type> eligibleImplementationTypes)
    {
        yield return new ServiceImplementationDeclaration(typeof(TImplementation), typeof(TService));
    }
}

/// <summary>
/// Marks an interface or a class to be an application service, declared at assembly level.
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <typeparam name="TService">The service type.</typeparam>
/// <typeparam name="TImplementation">The implementation type.</typeparam>
/// <typeparam name="TMetadata">The metadata type.</typeparam>
public abstract class DeclareServiceAttributeBase<TService, TImplementation, TMetadata>
    : DeclareServiceAttributeBase<TService, TImplementation>
    where TImplementation : TService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeclareServiceAttributeBase{TService,TImplementation,TMetadata}"/> class.
    /// </summary>
    /// <param name="lifetime">The lifetime.</param>
    protected DeclareServiceAttributeBase(AppServiceLifetime lifetime)
        : base(lifetime) =>
        MetadataType = typeof(TMetadata);
}