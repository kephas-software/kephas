// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclareTransientServiceAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection;

/// <summary>
/// Marks an interface or a class to be a transient application service, declared at assembly level.
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <typeparam name="TService">The service type.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class DeclareTransientServiceAttribute<TService>()
    : DeclareServiceAttributeBase<TService>(AppServiceLifetime.Transient);

/// <summary>
/// Marks an interface or a class to be a transient application service, declared at assembly level.
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <typeparam name="TService">The service type.</typeparam>
/// <typeparam name="TImplementation">The implementation type.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class DeclareTransientServiceAttribute<TService, TImplementation>()
    : DeclareServiceAttributeBase<TService, TImplementation>(AppServiceLifetime.Transient);

/// <summary>
/// Marks an interface or a class to be a transient application service, declared at assembly level.
/// This is useful when the service to be annotated is not accessible for modification.
/// </summary>
/// <typeparam name="TService">The service type.</typeparam>
/// <typeparam name="TImplementation">The implementation type.</typeparam>
/// <typeparam name="TMetadata">The metadata type.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class DeclareTransientServiceAttribute<TService, TImplementation, TMetadata>()
    : DeclareServiceAttributeBase<TService, TImplementation, TMetadata>(AppServiceLifetime.Transient);