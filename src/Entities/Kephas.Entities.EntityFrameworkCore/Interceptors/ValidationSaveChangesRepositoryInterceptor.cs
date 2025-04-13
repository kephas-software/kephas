// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationSaveChangesRepositoryInterceptor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;
using Kephas.Validation;
using Microsoft.EntityFrameworkCore;

namespace Kephas.Entities.Interceptors;

/// <summary>
/// An <see cref="ISaveChangesRepositoryInterceptor"/> to validate the entities.
/// </summary>
/// <remarks>
/// Declare a low priority, as other interceptors might do value initialization,
/// and the validator should be called last.
/// </remarks>
[Priority(Priority.Low)]
public class ValidationSaveChangesRepositoryInterceptor(IValidationService validationService)
    : ISaveChangesRepositoryInterceptor
{
    /// <summary>
    /// Invoked when saving the changes is executed with the provided arguments.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="entityContext">The affected entity context.</param>
    /// <param name="next">The next invocation in the execution pipeline.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>An awaitable task yielding the expected response.</returns>
    public async Task<int> HandleAsync(IWriteRepository repository, IEntityContext entityContext, Func<Task<int>> next,
        CancellationToken cancellationToken)
    {
        var dbContext = (DbContext)entityContext;
        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            await validationService.EnsureValidAsync(entry.Entity, cancellationToken);
        }
        
        return await next();
    }
}