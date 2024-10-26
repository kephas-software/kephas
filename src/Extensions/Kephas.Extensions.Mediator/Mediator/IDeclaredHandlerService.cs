// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHandlerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Extensions.Mediator;

/// <summary>
/// Marker interface for services registered through <see cref="DeclareTransientServiceFamilyAttribute{TService}"/>.
/// </summary>
public interface IDeclaredHandlerService;
