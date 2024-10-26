// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMediatorServiceProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Extensions.Mediator;

/// <summary>
/// Service provider for mediator.
/// </summary>
[TransientService]
public interface IMediatorServiceProvider : IServiceProvider;
