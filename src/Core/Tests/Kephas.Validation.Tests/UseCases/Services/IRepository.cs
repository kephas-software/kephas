// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Tests.UseCases.Services;

[ScopedService]
public interface IRepository
{
    Task<T> GetAsync<T>(CancellationToken cancellationToken) where T : new();
}
