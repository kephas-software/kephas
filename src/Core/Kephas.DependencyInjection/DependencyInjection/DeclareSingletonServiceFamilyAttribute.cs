// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclareSingletonServiceFamilyAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection;

/// <summary>
/// Marks an interface or a class to be a singleton application service, declared at assembly level.
/// All non-abstract types derived from the service type will be marked as service implementations (the service family).
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <remarks>
/// Open generics are not supported when declaring service families. In these cases, declare the services using:
/// <see cref="DeclareSingletonServiceAttribute"/>.
/// </remarks>
/// <param name="serviceDeclarationType">The service declaration type.</param>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class DeclareSingletonServiceFamilyAttribute(Type serviceDeclarationType)
    : DeclareServiceFamilyAttributeBase(serviceDeclarationType, AppServiceLifetime.Singleton);

/// <summary>
/// Marks the <typeparamref name="TService"/> as a singleton application service, declared at assembly level.
/// All non-abstract types derived from <typeparamref name="TService"/> will be marked as service implementations (the service family).
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <remarks>
/// Open generics are not supported when declaring service families. In these cases, declare the services using
/// <see cref="DeclareSingletonServiceAttribute"/>.
/// </remarks>
/// <typeparam name="TService">The service type.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class DeclareSingletonServiceFamilyAttribute<TService>()
    : DeclareServiceFamilyAttributeBase<TService>(AppServiceLifetime.Singleton);

/// <summary>
/// Marks the <typeparamref name="TService"/> as a singleton application service, declared at assembly level.
/// All non-abstract types derived from <typeparamref name="TService"/> will be marked as service implementations (the service family).
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <remarks>
/// Open generics are not supported when declaring service families. In these cases, declare the services using
/// <see cref="DeclareSingletonServiceAttribute"/>.
/// </remarks>
/// <typeparam name="TService">The service type.</typeparam>
/// <typeparam name="TMetadata">The metadata type.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class DeclareSingletonServiceFamilyAttribute<TService, TMetadata>()
    : DeclareServiceFamilyAttributeBase<TService, TMetadata>(AppServiceLifetime.Singleton);