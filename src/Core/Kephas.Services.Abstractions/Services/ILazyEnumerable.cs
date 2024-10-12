// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILazyEnumerable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ILazyEnumerable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Kephas.Services;

/// <summary>
/// Interface for ordered lazy service collection.
/// </summary>
/// <typeparam name="TContract">Type of the service contract.</typeparam>
/// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
[AppServiceContract(AsOpenGeneric = true)]
public interface ILazyEnumerable<TContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>
    : IEnumerable<Lazy<TContract, TMetadata>>
{
}
