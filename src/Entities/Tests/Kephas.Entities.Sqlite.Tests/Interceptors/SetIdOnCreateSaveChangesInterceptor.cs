// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetIdOnCreateSaveChangesRepositoryInterceptor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Entities.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Kephas.Entities.Tests.Interceptors;

public class SetIdOnCreateSaveChangesRepositoryInterceptor : ISaveChangesRepositoryInterceptor
{
    public Task<int> HandleAsync(IWriteRepository repository, IEntityContext entityContext, Func<Task<int>> next, CancellationToken cancellationToken)
    {
        var dbContext = (DbContext)entityContext;
        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            if (entry.Entity is IIdentifiable<Guid> identifiable && identifiable.Id == Guid.Empty)
            {
                identifiable.Id = Guid.NewGuid();
            }
        }

        return next();
    }
}