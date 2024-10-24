// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceDeclaration.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Kephas.DependencyInjection.ServiceProviders
{
    /// <summary>
    /// Service contract declaration record.
    /// </summary>
    /// <param name="ServiceDeclarationType">
    /// The type declaring the service type.
    /// This is the type annotated with one of the: [<see cref="TransientServiceAttribute"/>], [<see cref="ScopedServiceAttribute"/>],
    /// or [<see cref="SingletonServiceAttribute"/>], and which declares the actual service type.
    /// </param>
    /// <param name="MetadataType">The metadata type associated to the service type.</param>
    public record ServiceDeclaration(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type ServiceDeclarationType,
        Type? MetadataType = null);
}