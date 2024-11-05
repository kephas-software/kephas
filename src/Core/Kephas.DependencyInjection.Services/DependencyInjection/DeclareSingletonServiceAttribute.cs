// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclareSingletonServiceAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection;

/// <summary>
/// Marks an interface or a class to be a singleton application service, declared at assembly level.
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <param name="serviceType">The service type.</param>
/// <param name="implementationType">The implementation type.</param>
/// <param name="registrationKind">The DI container registration kind.</param>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class DeclareSingletonServiceAttribute(Type serviceType, Type implementationType, RegistrationKind registrationKind = RegistrationKind.Default)
    : DeclareServiceAttributeBase(serviceType, implementationType, AppServiceLifetime.Singleton, registrationKind);

/// <summary>
/// Marks an interface or a class to be a singleton application service, declared at assembly level.
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <typeparam name="TService">The service type.</typeparam>
/// <typeparam name="TImplementation">The implementation type.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class DeclareSingletonServiceAttribute<TService, TImplementation>()
    : DeclareServiceAttributeBase<TService, TImplementation>(AppServiceLifetime.Singleton)
    where TImplementation : TService;

/// <summary>
/// Marks an interface or a class to be a singleton application service, declared at assembly level.
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <typeparam name="TService">The service type.</typeparam>
/// <typeparam name="TImplementation">The implementation type.</typeparam>
/// <typeparam name="TMetadata">The metadata type.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class DeclareSingletonServiceAttribute<TService, TImplementation, TMetadata>()
    : DeclareServiceAttributeBase<TService, TImplementation, TMetadata>(AppServiceLifetime.Singleton)
    where TImplementation : TService;