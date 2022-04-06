// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjectable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection;

using Kephas.Services;

/// <summary>
/// Marker interface for classes instantiable through injection, for example using the <see cref="IInjectableFactory"/> service.
/// </summary>
public interface IInjectable
{
}