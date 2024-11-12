// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFactoryEnumerable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IFactoryEnumerable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Kephas.DependencyInjection.Metadata;

namespace Kephas.DependencyInjection;

/// <summary>
/// Interface for ordered service factory collection.
/// </summary>
/// <typeparam name="TService">The service type.</typeparam>
[TransientService]
public interface IFactoryEnumerable<out TService>
    : IEnumerable<IFactory<TService, ServiceMetadata>>
    where TService : notnull
{
}

/// <summary>
/// Interface for ordered service factory collection.
/// </summary>
/// <typeparam name="TService">The service type.</typeparam>
/// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
[TransientService]
public interface IFactoryEnumerable<out TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] out TMetadata>
    : IEnumerable<IFactory<TService, TMetadata>>
    where TService : notnull
{
}
