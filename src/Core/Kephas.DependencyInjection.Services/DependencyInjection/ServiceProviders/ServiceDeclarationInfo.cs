// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceDeclarationInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Kephas.DependencyInjection.Reflection;

namespace Kephas.DependencyInjection.ServiceProviders;

/// <summary>
/// Provides the <see cref="ServiceDeclarationType"/> with the associated <see cref="ServiceInfo"/>.
/// </summary>
/// <param name="ServiceDeclarationType">
/// The type declaring the service type.
/// This is the type annotated with one of the: [<see cref="TransientServiceAttribute"/>], [<see cref="ScopedServiceAttribute"/>],
/// or [<see cref="SingletonServiceAttribute"/>], and which declares the actual service type.
/// </param>
/// <param name="ServiceInfo">The <see cref="IServiceInfo"/> associated to the service declaration type.</param>
public record ServiceDeclarationInfo(
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type ServiceDeclarationType,
    IServiceInfo ServiceInfo);