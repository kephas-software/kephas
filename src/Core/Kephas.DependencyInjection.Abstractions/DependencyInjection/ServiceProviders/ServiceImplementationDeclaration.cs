// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceImplementationDeclaration.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Kephas.DependencyInjection.ServiceProviders
{
    /// <summary>
    /// Provides the service <see cref="ImplementationType"/> with the associated <see cref="ServiceDeclarationType"/>.
    /// </summary>
    /// <param name="ImplementationType">The service implementation type.</param>
    /// <param name="ServiceDeclarationType">
    /// The type declaring the service type.
    /// This is the type annotated with one of the: [<see cref="TransientServiceAttribute"/>], [<see cref="ScopedServiceAttribute"/>],
    /// or [<see cref="SingletonServiceAttribute"/>], and which declares the actual service type.
    /// </param>
    public record ServiceImplementationDeclaration(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type ImplementationType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type ServiceDeclarationType);
}