// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientQueryExecutorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the client query executor base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

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
    using Kephas.Model;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for client query processors.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultClientQueryProcessor : IClientQueryProcessor
    {
        /// <summary>
        /// Gets the generic method of <see cref="ExecuteQueryAsync{TClientEntity,TEntity}"/>.
        /// </summary>
        private static readonly MethodInfo ExecuteQueryAsyncMethod =
            ReflectionHelper.GetGenericMethodOf(
                _ => ((DefaultClientQueryProcessor)null).ExecuteQueryAsync<string, string>(null, null, CancellationToken.None));

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultClientQueryProcessor"/> class.
        /// </summary>
        /// <param name="injectableFactory">The injectable factory.</param>
        /// <param name="clientQueryConverter">The client query converter.</param>
        /// <param name="conversionService">The conversion service.</param>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="projectedTypeResolver">The projected type resolver.</param>
        /// <param name="dataSpaceFactory">The data space factory.</param>
        /// <param name="typeRegistry">The type registry.</param>
        public DefaultClientQueryProcessor(
            IInjectableFactory injectableFactory,
            IClientQueryConverter clientQueryConverter,
            IDataConversionService conversionService,
            ITypeResolver typeResolver,
            IProjectedTypeResolver projectedTypeResolver,
            IExportFactory<IDataSpace> dataSpaceFactory,
            IRuntimeTypeRegistry typeRegistry)
        {
            injectableFactory = injectableFactory ?? throw new ArgumentNullException(nameof(injectableFactory));
            clientQueryConverter = clientQueryConverter ?? throw new System.ArgumentNullException(nameof(clientQueryConverter));
            conversionService = conversionService ?? throw new ArgumentNullException(nameof(conversionService));
            typeResolver = typeResolver ?? throw new ArgumentNullException(nameof(typeResolver));
            projectedTypeResolver = projectedTypeResolver ?? throw new System.ArgumentNullException(nameof(projectedTypeResolver));
            dataSpaceFactory = dataSpaceFactory ?? throw new System.ArgumentNullException(nameof(dataSpaceFactory));

            this.DataSpaceFactory = dataSpaceFactory;
            this.TypeRegistry = typeRegistry;
            this.InjectableFactory = injectableFactory;
            this.ClientQueryConverter = clientQueryConverter;
            this.ConversionService = conversionService;
            this.TypeResolver = typeResolver;
            this.ProjectedTypeResolver = projectedTypeResolver;
        }

        /// <summary>
        /// Gets the data space factory.
        /// </summary>
        public IExportFactory<IDataSpace> DataSpaceFactory { get; }

        /// <summary>
        /// Gets the type registry.
        /// </summary>
        public IRuntimeTypeRegistry TypeRegistry { get; }

        /// <summary>
        /// Gets the context factory.
        /// </summary>
        /// <value>
        /// The context factory.
        /// </value>
        public IInjectableFactory InjectableFactory { get; }

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
        /// Gets the projected type resolver.
        /// </summary>
        /// <value>
        /// The projected type resolver.
        /// </value>
        public IProjectedTypeResolver ProjectedTypeResolver { get; }

        /// <summary>
        /// Executes the query asynchronously.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="optionsConfig">Optional. The configuration options.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the list of client entities.
        /// </returns>
        public async Task<IList<object>> ExecuteQueryAsync(
            ClientQuery query,
            Action<IClientQueryExecutionContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            var clientEntityType = this.TypeResolver.ResolveType(query.EntityType, throwOnNotFound: false);
            var entityType = this.ResolveEntityType(clientEntityType);
            using (var executionContext = this.CreateExecutionContext(optionsConfig))
            {
                executionContext.EntityType = entityType;
                executionContext.ClientEntityType = clientEntityType;

                var executeQueryMethod = ExecuteQueryAsyncMethod.MakeGenericMethod(clientEntityType, entityType);
                var asyncResult = (Task<IList<object>>)executeQueryMethod.Call(this, query, executionContext, cancellationToken);
                var clientEntities = await asyncResult.PreserveThreadContext();
                return clientEntities;
            }
        }

        /// <summary>
        /// Creates the execution context.
        /// </summary>
        /// <param name="optionsConfig">Optional. The configuration options.</param>
        /// <returns>
        /// The new execution context.
        /// </returns>
        protected virtual IClientQueryExecutionContext CreateExecutionContext(Action<IClientQueryExecutionContext> optionsConfig = null)
        {
            var context = this.InjectableFactory.Create<ClientQueryExecutionContext>();
            optionsConfig?.Invoke(context);
            return context;
        }

        /// <summary>
        /// Resolves the entity type based on its client counterpart.
        /// </summary>
        /// <param name="clientEntityType">Type of the client entity.</param>
        /// <returns>
        /// A type representing the entity type.
        /// </returns>
        protected virtual Type ResolveEntityType(Type clientEntityType)
        {
            return this.ProjectedTypeResolver.ResolveProjectedType(clientEntityType);
        }

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

            using var dataSpace = this.DataSpaceFactory.CreateInitializedValue(executionContext);
            var dataContext = dataSpace[executionContext.EntityType];
            var queryConversionContext = new ClientQueryConversionContext(dataContext)
            {
                Options = executionContext.Options,
            };
            executionContext.QueryConversionConfig?.Invoke(queryConversionContext);
            var query = (IQueryable<TEntity>)this.ClientQueryConverter.ConvertQuery(clientQuery, queryConversionContext);
            var entities = await query.ToListAsync(token).PreserveThreadContext();

            // skip the conversion if the same types provided
            if (typeof(TClientEntity) == typeof(TEntity))
            {
                return entities.Cast<object>().ToList();
            }

            var clientEntityTypeInfo = this.TypeRegistry.GetTypeInfo(typeof(TClientEntity));
            foreach (var entity in entities)
            {
                using var context = new DataConversionContext(dataSpace, this.ConversionService)
                    .RootTargetType(clientEntityTypeInfo.Type);
                executionContext?.DataConversionConfig?.Invoke(entity, context);
                var result = await this.ConversionService.ConvertAsync(entity, clientEntityTypeInfo.CreateInstance(), context, token).PreserveThreadContext();
                mappings.Add(((TClientEntity)result.Target, entity));
            }

            return mappings.Select(m => (object)m.clientEntity).ToArray();
        }
    }
}