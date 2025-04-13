// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqlReadRepository.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Entities;

/// <summary>
/// Base interface for repositories used in reading entities using EntityFrameworkCore.
/// </summary>
[ScopedService]
public interface ISqlReadRepository : IReadRepository;