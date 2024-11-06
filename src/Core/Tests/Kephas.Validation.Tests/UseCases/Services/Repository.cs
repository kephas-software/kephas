// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Repository.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.UseCases.Services;

public class Repository : IRepository
{
    public async Task<T> GetAsync<T>(CancellationToken cancellationToken) where T : new()
    {
        await Task.Delay(100, cancellationToken);
        return new T();
    }
}
