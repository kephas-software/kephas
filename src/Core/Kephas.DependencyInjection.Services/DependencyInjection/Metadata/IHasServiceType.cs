// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHasServiceType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Kephas.DependencyInjection.Metadata;

/// <summary>
/// Interface providing the <see cref="ServiceType"/>.
/// </summary>
public interface IHasServiceType
{
    /// <summary>
    /// Gets or sets the type used to register and retrieve the service. Typically, this is an interface.
    /// </summary>
    public Type? ServiceType
    {
        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        get;
    }
}