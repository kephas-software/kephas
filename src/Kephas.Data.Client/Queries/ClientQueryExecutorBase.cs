// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientQueryExecutorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the client query executor base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Client.Queries.Conversion;
    using Kephas.Data.Conversion;
    using Kephas.Data.Linq;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for client query executors.
    /// </summary>
    public abstract class ClientQueryExecutorBase : IClientQueryExecutor
    {
        /// <summary>
        /// Gets the generic method of <see cref="ExecuteQueryAsync{TClientEntity,TEntity}"/>.
        /// </summary>
        private static readonly MethodInfo ExecuteQueryAsyncMethod =
            ReflectionHelper.GetGenericMethodOf(
                _ => ((ClientQueryExecutorBase)null).ExecuteQueryAsync<string, string>(null, null, CancellationToken.None));

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientQueryExecutorBase"/> class.
        /// </summary>
        /// <param name="clientQueryConverter">The client query converter.</param>
        /// <param name="conversionService">The conversion service.</param>
        /// <param name="typeResolver">The type resolver.</param>
        protected ClientQueryExecutorBase(
            IClientQueryConverter clientQueryConverter,
            IDataConversionService conversionService,
            ITypeResolver typeResolver)
        {
            Requires.NotNull(clientQueryConverter, nameof(clientQueryConverter));
            Requires.NotNull(conversionService, nameof(conversionService));

            this.ClientQueryConverter = clientQueryConverter;
            this.ConversionService = conversionService;
            this.TypeResolver = typeResolver;
        }

        /// <summary>
        /// Gets the client query converter.
        /// </summary>
        /// <value>
        /// The client query converter.
        /// </value>
        public IClientQueryConverter ClientQueryConverter { get; }

        /// <summary>
        /// Gets the conversion service.
        /// </summary>
        /// <value>
        /// The conversion service.
        /// </value>
        public IDataConversionService ConversionService { get; }

        /// <summary>
        /// Gets the type resolver.
        /// </summary>
        /// <value>
        /// The type resolver.
        /// </value>
        public ITypeResolver TypeResolver { get; }

        /// <summary>
        /// Executes the query asynchronously.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="executionContext">Context for the execution (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A list of client entities.
        /// </returns>
        public async Task<IList<object>> ExecuteQueryAsync(
            ClientQuery query,
            IClientQueryExecutionContext executionContext = null,
            CancellationToken cancellationToken = default)
        {
            var clientEntityType = this.TypeResolver.ResolveType(query.EntityType, throwOnNotFound: false);
            var entityType = this.ResolveEntityType(clientEntityType);
            executionContext = executionContext ?? new ClientQueryExecutionContext(this.ConversionService.AmbientServices);

            var executeQueryMethod = ExecuteQueryAsyncMethod.MakeGenericMethod(clientEntityType, entityType);
            var asyncResult = (Task<IList<object>>)executeQueryMethod.Call(this, query, executionContext, cancellationToken);
            var clientEntities = await asyncResult.PreserveThreadContext();
            return clientEntities;
        }

        /// <summary>
        /// Resolves the entity type based on its client counterpart.
        /// </summary>
        /// <param name="clientEntityType">Type of the client entity.</param>
        /// <returns>
        /// A type representing the entity type.
        /// </returns>
        protected abstract Type ResolveEntityType(Type clientEntityType);

        /// <summary>
        /// Creates the client data context.
        /// </summary>
        /// <returns>
        /// The new client data context.
        /// </returns>
        protected abstract IDataContext CreateClientDataContext();

        /// <summary>
        /// Creates the entity data context.
        /// </summary>
        /// <returns>
        /// The new data context.
        /// </returns>
        protected abstract IDataContext CreateDataContext();

        /// <summary>
        /// Executes the query operation.
        /// </summary>
        /// <typeparam name="TClientEntity">Type of the client entity.</typeparam>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="clientQuery">The client query.</param>
        /// <param name="executionContext">Context for the execution.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// A list of.
        /// </returns>
        protected virtual async Task<IList<object>> ExecuteQueryAsync<TClientEntity, TEntity>(
            ClientQuery clientQuery,
            IClientQueryExecutionContext executionContext,
            CancellationToken token)
            where TClientEntity : class
            where TEntity : class
        {
            var mappings = new List<(TClientEntity clientEntity, TEntity entity)>();

            using (var dataContext = this.CreateDataContext())
            using (var clientDataContext = this.CreateClientDataContext())
            {
                var queryConversionContext = new ClientQueryConversionContext(dataContext);
                executionContext?.ClientQueryConversionContextConfig?.Invoke(queryConversionContext);
                var query = (IQueryable<TEntity>)this.ClientQueryConverter.ConvertQuery(clientQuery, queryConversionContext);
                var domainEntities = await query.ToListAsync(token).PreserveThreadContext();

                var clientEntityTypeInfo = typeof(TClientEntity).AsRuntimeTypeInfo();
                foreach (var entity in domainEntities)
                {
                    var context = new DataConversionContext(this.ConversionService, sourceDataContext: dataContext, targetDataContext: clientDataContext, rootTargetType: clientEntityTypeInfo.Type);
                    executionContext?.DataConversionContextConfig?.Invoke(entity, context);
                    var result = await this.ConversionService.ConvertAsync(entity, clientEntityTypeInfo.CreateInstance(), context, token).PreserveThreadContext();
                    mappings.Add(((TClientEntity)result.Target, entity));
                }
            }

            return mappings.Select(m => (object)m.clientEntity).ToArray();
        }
    }
}