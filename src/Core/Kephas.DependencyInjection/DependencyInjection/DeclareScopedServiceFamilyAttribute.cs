// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclareScopedServiceFamilyAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection;

/// <summary>
/// Marks an interface or a class to be a scoped application service, declared at assembly level.
/// All non-abstract types derived from the service type will be marked as service implementations (the service family).
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <remarks>
/// Open generics are not supported when declaring service families. In these cases, declare the services using 
/// <see cref="DeclareScopedServiceAttribute"/>.
/// </remarks>
/// <param name="serviceDeclarationType">The service declaration type.</param>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class DeclareScopedServiceFamilyAttribute(Type serviceDeclarationType)
    : DeclareServiceFamilyAttributeBase(serviceDeclarationType, AppServiceLifetime.Scoped);

/// <summary>
/// Marks the <typeparamref name="TService"/> as a scoped application service, declared at assembly level.
/// All non-abstract types derived from <typeparamref name="TService"/> will be marked as service implementations (the service family).
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <remarks>
/// Open generics are not supported when declaring service families. In these cases, declare the services using
/// <see cref="DeclareScopedServiceAttribute"/>.
/// </remarks>
/// <typeparam name="TService">The service type.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class DeclareScopedServiceFamilyAttribute<TService>()
    : DeclareServiceFamilyAttributeBase<TService>(AppServiceLifetime.Scoped);

/// <summary>
/// Marks the <typeparamref name="TService"/> as a scoped application service, declared at assembly level.
/// All non-abstract types derived from <typeparamref name="TService"/> will be marked as service implementations (the service family).
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <remarks>
/// Open generics are not supported when declaring service families. In these cases, declare the services using 
/// <see cref="DeclareScopedServiceAttribute"/>.
/// </remarks>
/// <typeparam name="TService">The service type.</typeparam>
/// <typeparam name="TMetadata">The metadata type.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class DeclareScopedServiceFamilyAttribute<TService, TMetadata>()
    : DeclareServiceFamilyAttributeBase<TService, TMetadata>(AppServiceLifetime.Scoped);