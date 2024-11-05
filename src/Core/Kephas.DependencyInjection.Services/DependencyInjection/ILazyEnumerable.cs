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
using Kephas.DependencyInjection.Metadata;

namespace Kephas.DependencyInjection;

/// <summary>
/// Interface for ordered lazy service collection.
/// </summary>
/// <typeparam name="TService">Type of the service.</typeparam>
public interface ILazyEnumerable<TService>
    : IEnumerable<Lazy<TService, ServiceMetadata>>
{
}

/// <summary>
/// Interface for ordered lazy service collection.
/// </summary>
/// <typeparam name="TService">Type of the service.</typeparam>
/// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
public interface ILazyEnumerable<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>
    : IEnumerable<Lazy<TService, TMetadata>>
{
}
