// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISaveChangesRepositoryInterceptor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using Kephas.Reflection;

namespace Kephas.Entities.Interceptors;

/// <summary>
/// Base interface for query operations.
/// </summary>
public interface IQueryOperation : IRepositoryOperation<IQueryable>
{
    /// <summary>
    /// Gets the entity context.
    /// </summary>
    IEntityContext EntityContext { get; }
    
    /// <summary>
    /// Gets the query item type.
    /// </summary>
    Type ItemType { get; }
}

/// <summary>
/// The execute operation arguments.
/// </summary>
/// <param name="Repository">The repository.</param>
/// <param name="EntityContext">The entity context being used for querying.</param>
/// <param name="ItemType">The query item type.</param>
public record QueryOperation(IReadRepository Repository, IEntityContext EntityContext, Type ItemType) : IQueryOperation;

/// <summary>
/// Interceptor being invoked when the changes are persisted for an entity context.
/// </summary>
public interface IQueryRepositoryInterceptor : IRepositorySyncInterceptor<IQueryOperation, IQueryable>
{
    private static readonly MethodInfo HandleMethod = ReflectionHelper.GetGenericMethodOf(_ => ((IQueryRepositoryInterceptor)null!).Handle<string>(null!, null!, null!));
    private static readonly MethodInfo ConvertNextMethod = ReflectionHelper.GetGenericMethodOf(_ => ConvertNext<string>(null!));
    
    /// <summary>
    /// Invoked when saving the changes is executed with the provided arguments.
    /// </summary>
    /// <param name="operation">The intercepted operation.</param>
    /// <param name="next">The next invocation in the execution pipeline.</param>
    /// <returns>An awaitable task yielding the expected response.</returns>
    IQueryable IRepositorySyncInterceptor<IQueryOperation, IQueryable>.Handle(
        IQueryOperation operation,
        InterceptSyncNextDelegate<IQueryable> next)
    {
        var handleMethod = HandleMethod.MakeGenericMethod(operation.ItemType);
        var convertNextMethod = ConvertNextMethod.MakeGenericMethod(operation.ItemType);
        var typedNext = convertNextMethod.Call(null, next);
        return handleMethod.Call<IQueryable>(this, operation.Repository, operation.EntityContext, typedNext);
    }

    /// <summary>
    /// Invoked when saving the changes is executed with the provided arguments.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="entityContext">The affected entity context.</param>
    /// <param name="next">The next invocation in the execution pipeline.</param>
    /// <returns>An awaitable task yielding the expected response.</returns>
    IQueryable<T> Handle<T>(IReadRepository repository, IEntityContext entityContext, Func<IQueryable<T>> next);
    
    private static Func<IQueryable<T>> ConvertNext<T>(InterceptSyncNextDelegate<IQueryable> next) => () => (IQueryable<T>)next();
}