// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFactoryEnumerable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IFactoryEnumerable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Interface for ordered service factory collection.
/// </summary>
/// <typeparam name="TContract">Type of the service contract.</typeparam>
/// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
[AppServiceContract(AsOpenGeneric = true)]
public interface IFactoryEnumerable<out TContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] out TMetadata>
    : IEnumerable<IExportFactory<TContract, TMetadata>>
{
}
