// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlWriteRepositoryBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using Kephas.Collections;
using Kephas.Entities.Interceptors;
using Kephas.Entities.StoredProcedures;
using Kephas.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kephas.Entities;

/// <summary>
/// The base implementation of <see cref="IWriteRepository"/> for EntityFrameworkCore.
/// </summary>
public abstract class SqlWriteRepositoryBase(
    ISqlEntityContextSelector entityContextSelector,
    IRepositoryOperationPipeline operationPipeline)
    : SqlReadRepositoryBase(entityContextSelector, operationPipeline), IWriteRepository
{
    private static readonly MethodInfo ExecuteAsyncMethod =
        ReflectionHelper.GetGenericMethodOf(_ => ((SqlWriteRepositoryBase)null!).ExecuteAsync<ISpArgs>(null!, default));
    
    private readonly HashSet<DbContext> activeDbContexts = new();
    
    private bool hasTransaction;

#region Transactions

    /// <summary>
    /// Gets a value indicating whether a transaction is active.
    /// </summary>
    public bool HasTransaction() => hasTransaction;

    /// <summary>
    /// Executes the operation inside a new transaction.
    /// </summary>
    async Task<T> IWriteRepository.EnterNewTransactionAsyncCore<T>(
        Func<IWriteRepository, CancellationToken, T> operation,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(operation);

        Func<Task<T>> next = null!;
        using var enumerator = entityContextSelector.GetEnumerator();
        next = async () =>
        {
            if (enumerator.MoveNext())
            {
                var dbContext = (DbContext)enumerator.Current;
                return await EnterTransactionAsync(dbContext, next, cancellationToken);
            }

            var task = operation(this, cancellationToken);
            await task;
            return task;
        };

        try
        {
            hasTransaction = true;
            return await next();
        }
        finally
        {
            hasTransaction = false;
        }
    }

    private async Task<T> EnterTransactionAsync<T>(DbContext dbContext, Func<Task<T>> next, CancellationToken cancellationToken)
        where T : Task
    {
        var database = dbContext.Database; 
        var strategy = database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(
            async token =>
            {
                var (transaction, existing) = await BeginTransactionAsync(database, token);
                try
                {
                    var result = await next();
                    await transaction.CommitAsync(token);
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(token);
                    throw;
                }
                finally
                {
                    if (!existing)
                    {
                        transaction.Dispose();
                    }
                }
            }, cancellationToken);
    }

    private static async Task<(IDbContextTransaction transaction, bool existing)>
        BeginTransactionAsync(DatabaseFacade database, CancellationToken token) =>
        database.CurrentTransaction is not null
            ? (database.CurrentTransaction, true)
            : (await database.BeginTransactionAsync(token), false);

    #endregion
    
    /// <summary>
    /// Executes the stored procedure with the provided arguments.
    /// </summary>
    /// <param name="args">The stored procedure arguments. Their type is annotated with [<see cref="StoredProcedureAttribute"/>].</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A task. Output parameters are retrieved in the provided <paramref name="args"/>.</returns>
    public virtual Task ExecuteAsync(ISpArgs args, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(args);

        var executeAsync = ExecuteAsyncMethod.MakeGenericMethod(args.GetType());
        return executeAsync.Call<Task>(this, args, cancellationToken);
    }

    /// <summary>
    /// Attaches the entity, making the change tracker start tracking its changes.
    /// </summary>
    /// <param name="entity">The entity to attach.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    public void Attach<T>(T entity) where T : class
    {
        ArgumentNullException.ThrowIfNull(entity);
        GetDbContext(typeof(T)).Attach(entity);
    }

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    public void Add<T>(T entity) where T : class
    {
        ArgumentNullException.ThrowIfNull(entity);
        Set<T>().Add(entity);
    }

    /// <summary>
    /// Update an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    public void Update<T>(T entity) where T : class
    {
        ArgumentNullException.ThrowIfNull(entity);
        Set<T>().Update(entity);
    }

    /// <summary>
    /// Delete an existing entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    public void Delete<T>(T entity) where T : class
    {
        ArgumentNullException.ThrowIfNull(entity);
        GetDbContext(typeof(T)).Remove(entity);
    }

    /// <summary>
    /// Tries to delete an existing entity from the repository.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns><c>true</c> if the entity was found, otherwise <c>false</c>.</returns>
    public bool TryDelete<T>(object id) where T : class
    {
        var entity = Set<T>().Find(id);
        if (entity is not null)
        {
            GetDbContext(typeof(T)).Remove(entity);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Saves the changes asynchronously to the persistence layer.
    /// </summary>
    /// <remarks>Calling this method requires an enclosing transaction.</remarks>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A task.</returns>
    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = 0;
        foreach (var dbContext in activeDbContexts)
        {
            result += await SaveChangesAsync(dbContext, cancellationToken);
        }

        return result;
    }

    /// <summary>
    /// Saves the changes asynchronously to the persistence layer.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/> to save.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A task.</returns>
    protected virtual Task<int> SaveChangesAsync(DbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (!dbContext.ChangeTracker.HasChanges())
        {
            return Task.FromResult(0);
        }

        var saveChangesOperation = new SaveChangesOperation(this, (IEntityContext)dbContext);
        return operationPipeline.HandleAsync(
            saveChangesOperation,
            () => dbContext.SaveChangesAsync(cancellationToken),
            cancellationToken);
    }

    /// <summary>
    /// Executes the stored procedure with the provided arguments.
    /// </summary>
    /// <param name="args">The stored procedure arguments. Their type is annotated with [<see cref="StoredProcedureAttribute"/>].</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A task. Output parameters are retrieved in the provided <paramref name="args"/>.</returns>
    protected virtual async Task ExecuteAsync<TSpArgs>(TSpArgs args, CancellationToken cancellationToken = default)
        where TSpArgs : ISpArgs
    {
        ArgumentNullException.ThrowIfNull(args);

        var spName = typeof(TSpArgs).GetCustomAttribute<StoredProcedureAttribute>()?.Name
                     ?? throw new InvalidOperationException($"The {typeof(TSpArgs)} has no '{nameof(StoredProcedureAttribute)}' attribute.");

        var spArgs = args.GetType()
            .GetPublicInstanceProperties()
            .Select(p => new SpParameter(p.Name, p.GetValue(args), p.PropertyType, p.GetCustomAttribute<SpParameterAttribute>()))
            .ToList();

        var spCommandText = GetSpCommandText(spName, spArgs);

        var sqlParams = GetSqlParameters(spArgs).ToArray();

        var executeArgs = new ExecuteOperation<TSpArgs>(this, args);
        await operationPipeline.HandleAsync(
            executeArgs,
            async () =>
            {
                await ExecuteCoreAsync<TSpArgs>(spCommandText, sqlParams, cancellationToken);
                SyncOutputArgs(args, sqlParams);
                return null;
            },
            cancellationToken);
    }

    /// <summary>
    /// Executes the stored procedure command asynchronously.
    /// </summary>
    /// <param name="spCommand">The stored procedure command text.</param>
    /// <param name="sqlParams">The SQL parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task.</returns>
    protected virtual Task ExecuteCoreAsync<TArgs>(string spCommand, IEnumerable<IDbDataParameter> sqlParams, CancellationToken cancellationToken)
    {
        var database = GetDatabase(typeof(TArgs));
        return database.ExecuteSqlRawAsync(spCommand, sqlParams, cancellationToken);
    }

    /// <summary>
    /// Gets the <see cref="DbContext"/> used by this repository to access data.
    /// </summary>
    /// <param name="entityType">The entity type for which the data context is required.</param>
    protected override DbContext GetDbContext(Type entityType)
    {
        var dataContext = base.GetDbContext(entityType);
        activeDbContexts.Add(dataContext);

        return dataContext;
    }

    #region Stored Procedures

    private static readonly Dictionary<Type, DbType> DbTypeMap = new()
    {
        { typeof(string), DbType.String },
        { typeof(Guid), DbType.Guid },
        { typeof(Guid?), DbType.Guid },
        { typeof(DateTime), DbType.Date },
        { typeof(DateTime?), DbType.Date },
        { typeof(long), DbType.Int64 },
        { typeof(long?), DbType.Int64 },
        { typeof(int), DbType.Int32 },
        { typeof(int?), DbType.Int32 },
        { typeof(short), DbType.Int16 },
        { typeof(short?), DbType.Int16 },
        { typeof(byte), DbType.Byte },
        { typeof(byte?), DbType.Byte },
        { typeof(sbyte), DbType.SByte },
        { typeof(sbyte?), DbType.SByte },
        { typeof(decimal), DbType.Decimal },
        { typeof(decimal?), DbType.Decimal },
    };

    private IEnumerable<IDbDataParameter> GetSqlParameters(IEnumerable<SpParameter> spArgs)
    {
        foreach (var (name, value, type, options) in spArgs)
        {
            var sqlParam = GetSqlParameter(name, type, options, value);

            yield return sqlParam;
        }
    }

    /// <summary>
    /// Gets a <see cref="SqlParameter"/> out of the provided arguments.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The parameter managed type.</param>
    /// <param name="spArg">The stored procedure argument.</param>
    /// <param name="value">The argument value.</param>
    /// <returns>A <see cref="SqlParameter"/>.</returns>
    protected virtual SqlParameter GetSqlParameter(string name, Type type, ISpParameter? spArg, object? value)
    {
        var sqlParam = new SqlParameter
        {
            ParameterName = name,
            Direction = spArg?.Direction ?? ParameterDirection.Input,
            Value = value ?? DBNull.Value,
        };

        if (spArg?.DbType is null && spArg?.SqlDbType is null)
        {
            sqlParam.DbType = DbTypeMap.GetValueOrDefault(type, DbType.String);
        }
        else
        {
            if (spArg.DbType is not null)
            {
                sqlParam.DbType = spArg.DbType.Value;
            }

            if (spArg.SqlDbType is not null)
            {
                sqlParam.SqlDbType = spArg.SqlDbType.Value;
            }
        }

        if (spArg?.TypeName is not null)
        {
            sqlParam.TypeName = spArg.TypeName;
        }

        if (spArg?.Size is not null)
        {
            sqlParam.Size = spArg.Size.Value;
        }
        else if (sqlParam.Direction is ParameterDirection.Output or ParameterDirection.ReturnValue)
        {
            sqlParam.Size = -1;
        }

        if (type.IsNullableType() || (type.IsClass && value is null))
        {
            sqlParam.IsNullable = true;
        }

        return sqlParam;
    }

    private sealed record SpParameter(string Name, object? Value, Type Type, SpParameterAttribute? Options);

    private static string GetSpCommandText(string spName, List<SpParameter> spArgs)
    {
        var spBuilder = new StringBuilder(spName);
        foreach (var (key, _, _, options) in spArgs)
        {
            spBuilder.Append(" @").Append(key)
                .Append(options?.Direction is ParameterDirection.Output ? " OUTPUT" : string.Empty).Append(',');
        }

        spBuilder.Length--;
        return spBuilder.ToString();
    }

    private static void SyncOutputArgs<TArgs>([DisallowNull] TArgs args, IEnumerable<IDbDataParameter> sqlParams)
    {
        var dynamicArgs = args.ToDynamic();
        sqlParams
            .Where(p => p.Direction == ParameterDirection.Output)
            .ForEach(p => { dynamicArgs[p.ParameterName] = p.Value; });
    }

    #endregion
}