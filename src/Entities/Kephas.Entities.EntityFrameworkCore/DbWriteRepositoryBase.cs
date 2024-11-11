// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbWriteRepositoryBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Entities.StoredProcedures;

namespace Kephas.Entities;

/// <summary>
/// The base implementation of <see cref="IWriteRepository"/> for EntityFrameworkCore.
/// </summary>
public abstract class DbWriteRepositoryBase(IDbEntityContextSelector entityContextSelector)
    : DbReadRepositoryBase(entityContextSelector), IWriteRepository
{
    public async Task ExecuteAsync(ISpArgs args, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Attach<T>(T entity) where T : class
    {
        throw new NotImplementedException();
    }

    public void Add<T>(T entity) where T : class
    {
        throw new NotImplementedException();
    }

    public void Update<T>(T entity) where T : class
    {
        throw new NotImplementedException();
    }

    public void Delete<T>(T entity) where T : class
    {
        throw new NotImplementedException();
    }

    public bool TryDelete<T>(Guid id) where T : class
    {
        throw new NotImplementedException();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}