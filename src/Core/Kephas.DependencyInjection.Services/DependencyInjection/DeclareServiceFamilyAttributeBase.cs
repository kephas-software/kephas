// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclareServiceFamilyAttributeBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.ServiceProviders;
using Kephas.Reflection;

namespace Kephas.DependencyInjection;

/// <summary>
/// Marks an interface or a class to be an application service, declared at assembly level.
/// All non-abstract types derived from the service type will be marked as service implementations (the service family).
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <remarks>
/// Open generics are not supported when declaring service families. In these cases, declare the services using one of:
/// <see cref="DeclareTransientServiceAttribute"/>, <see cref="DeclareScopedServiceAttribute"/>, and <see cref="DeclareSingletonServiceAttribute"/>.
/// </remarks>
public abstract class DeclareServiceFamilyAttributeBase : ServiceAttributeBase, IServiceInfo, IServiceInfoProvider
{
    private readonly Type serviceDeclarationType;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeclareServiceFamilyAttributeBase"/> class.
    /// </summary>
    /// <param name="serviceDeclarationType">The service declaration type.</param>
    /// <param name="lifetime">The lifetime.</param>
    protected DeclareServiceFamilyAttributeBase(Type serviceDeclarationType, AppServiceLifetime lifetime)
        : base(lifetime, RegistrationKind.AsClosedGeneric)
    {
        this.serviceDeclarationType = serviceDeclarationType ?? throw new ArgumentNullException(nameof(serviceDeclarationType));
        ServiceType = serviceDeclarationType;
    }

    /// <summary>
    /// Gets the type declaring the service type.
    /// </summary>
    /// <remarks>
    /// This is the type annotated with one of the: [<see cref="TransientServiceAttribute"/>], [<see cref="ScopedServiceAttribute"/>],
    /// or [<see cref="SingletonServiceAttribute"/>], and which declares the actual service type.
    /// Typically, this is a generic type used for collecting metadata, redirecting to the non-generic service type.
    /// </remarks>
    Type? IServiceInfo.ServiceDeclarationType => serviceDeclarationType;

    /// <summary>
    /// Gets an enumeration of <see cref="ServiceDeclaration"/>.
    /// </summary>
    /// <returns>
    /// An enumeration of <see cref="ServiceDeclaration"/>.
    /// </returns>
    IEnumerable<ServiceDeclaration>? IServiceInfoProvider.GetServiceDeclarations()
    {
        yield return new ServiceDeclaration(serviceDeclarationType, MetadataType);
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
        yield return new ServiceDeclarationInfo(serviceDeclarationType, new ServiceInfo(ServiceType!, Lifetime, AsOpenGeneric) { MetadataType = MetadataType });
    }

    /// <summary>
    /// Gets an enumeration of <see cref="ServiceImplementationDeclaration"/>.
    /// </summary>
    /// <param name="eligibleImplementationTypes">The eligible implementation types.</param>
    /// <returns>
    /// An enumeration of <see cref="ServiceImplementationDeclaration"/>.
    /// </returns>
    public IEnumerable<ServiceImplementationDeclaration> GetImplementationDeclarations(IEnumerable<Type> eligibleImplementationTypes) =>
        serviceDeclarationType.GetImplementationDeclarations(ServiceType!, eligibleImplementationTypes);
}

/// <summary>
/// Marks the <typeparamref name="TService"/> as a scoped application service, declared at assembly level.
/// All non-abstract types derived from <typeparamref name="TService"/> will be marked as service implementations (the service family).
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <remarks>
/// Open generics are not supported when declaring service families. In these cases, declare the services using one of:
/// <see cref="DeclareTransientServiceAttribute"/>, <see cref="DeclareScopedServiceAttribute"/>, and <see cref="DeclareSingletonServiceAttribute"/>.
/// </remarks>
/// <typeparam name="TService">The service type.</typeparam>
public abstract class DeclareServiceFamilyAttributeBase<TService> : ServiceAttributeBase, IServiceInfo, IServiceInfoProvider
{
    private readonly Type serviceDeclarationType;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeclareServiceFamilyAttributeBase{TService}"/> class.
    /// </summary>
    /// <param name="lifetime">The lifetime.</param>
    protected DeclareServiceFamilyAttributeBase(AppServiceLifetime lifetime)
        : base(lifetime, RegistrationKind.AsClosedGeneric)
    {
        serviceDeclarationType = typeof(TService);
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
    Type? IServiceInfo.ServiceDeclarationType => serviceDeclarationType;

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
        yield return new ServiceDeclarationInfo(serviceDeclarationType, new ServiceInfo(ServiceType!, Lifetime, AsOpenGeneric) { MetadataType = MetadataType });
    }

    /// <summary>
    /// Gets an enumeration of <see cref="ServiceDeclaration"/>.
    /// </summary>
    /// <returns>
    /// An enumeration of <see cref="ServiceDeclaration"/>.
    /// </returns>
    IEnumerable<ServiceDeclaration>? IServiceInfoProvider.GetServiceDeclarations()
    {
        yield return new ServiceDeclaration(serviceDeclarationType, MetadataType);
    }

    /// <summary>
    /// Gets an enumeration of <see cref="ServiceImplementationDeclaration"/>.
    /// </summary>
    /// <param name="eligibleImplementationTypes">The eligible implementation types.</param>
    /// <returns>
    /// An enumeration of <see cref="ServiceImplementationDeclaration"/>.
    /// </returns>
    public IEnumerable<ServiceImplementationDeclaration> GetImplementationDeclarations(IEnumerable<Type> eligibleImplementationTypes) =>
        serviceDeclarationType.GetImplementationDeclarations(ServiceType!, eligibleImplementationTypes);
}

/// <summary>
/// Marks the <typeparamref name="TService"/> as a scoped application service, declared at assembly level.
/// All non-abstract types derived from <typeparamref name="TService"/> will be marked as service implementations (the service family).
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <remarks>
/// Open generics are not supported when declaring service families. In these cases, declare the services using one of:
/// <see cref="DeclareTransientServiceAttribute"/>, <see cref="DeclareScopedServiceAttribute"/>, and <see cref="DeclareSingletonServiceAttribute"/>.
/// </remarks>
/// <typeparam name="TService">The service type.</typeparam>
/// <typeparam name="TMetadata">The metadata type.</typeparam>
public abstract class DeclareServiceFamilyAttributeBase<TService, TMetadata>
    : DeclareServiceFamilyAttributeBase<TService>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeclareServiceFamilyAttributeBase{TService, TMetadata}"/> class.
    /// </summary>
    /// <param name="lifetime">The lifetime.</param>
    protected DeclareServiceFamilyAttributeBase(AppServiceLifetime lifetime) : base(lifetime) =>
        MetadataType = typeof(TMetadata);
}