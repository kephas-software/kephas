﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSourceHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data source handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Endpoints
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.DataSources;
    using Kephas.Dynamic;
    using Kephas.Services;
    using Kephas.Messaging;
    using Kephas.Model;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A data source handler.
    /// </summary>
    public class DataSourceHandler : IMessageHandler<DataSourceMessage, DataSourceResponse>
    {
        /// <summary>
        /// The data space factory.
        /// </summary>
        private readonly IExportFactory<IDataSpace> dataSpaceFactory;

        /// <summary>
        /// The data source service.
        /// </summary>
        private readonly IDataSourceService dataSourceService;

        /// <summary>
        /// The type resolver.
        /// </summary>
        private readonly ITypeResolver typeResolver;

        /// <summary>
        /// The projected type resolver.
        /// </summary>
        private readonly IProjectedTypeResolver projectedTypeResolver;

        private readonly IRuntimeTypeRegistry typeRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceHandler"/> class.
        /// </summary>
        /// <param name="dataSpaceFactory">The data space factory.</param>
        /// <param name="dataSourceService">The data source service.</param>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="projectedTypeResolver">The projected type resolver.</param>
        /// <param name="typeRegistry">The type registry.</param>
        public DataSourceHandler(
            IExportFactory<IDataSpace> dataSpaceFactory,
            IDataSourceService dataSourceService,
            ITypeResolver typeResolver,
            IProjectedTypeResolver projectedTypeResolver,
            IRuntimeTypeRegistry typeRegistry)
        {
            dataSpaceFactory = dataSpaceFactory ?? throw new System.ArgumentNullException(nameof(dataSpaceFactory));
            dataSourceService = dataSourceService ?? throw new System.ArgumentNullException(nameof(dataSourceService));
            typeResolver = typeResolver ?? throw new ArgumentNullException(nameof(typeResolver));
            projectedTypeResolver = projectedTypeResolver ?? throw new System.ArgumentNullException(nameof(projectedTypeResolver));

            this.dataSpaceFactory = dataSpaceFactory;
            this.dataSourceService = dataSourceService;
            this.typeResolver = typeResolver;
            this.projectedTypeResolver = projectedTypeResolver;
            this.typeRegistry = typeRegistry;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public async Task<DataSourceResponse> ProcessAsync(DataSourceMessage message, IMessagingContext context, CancellationToken token)
        {
            var entityType = this.ResolveEntityType(message.EntityType, context);
            var property = this.ResolveProperty(entityType, message.Property);

            using var dataSpace = this.dataSpaceFactory.CreateInitializedValue(context);

            var projectedType = this.ResolveProjectedEntityType(entityType, context);
            var dataContext = dataSpace[projectedType];
            var dataSourceContext = new DataSourceContext(dataContext, entityType, projectedType)
            {
                Options = message.Options,
            };
            dataSourceContext.Merge(message.Options);
            var dataSource = await this.dataSourceService.GetDataSourceAsync(
                property,
                dataSourceContext,
                token).PreserveThreadContext();

            return new DataSourceResponse
            {
                DataSource = dataSource?.ToList(),
            };
        }

        /// <summary>
        /// Resolve the entity type.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="entityTypeName">Name of the entity type.</param>
        /// <param name="context">The messaging context.</param>
        /// <returns>
        /// An ITypeInfo.
        /// </returns>
        protected virtual ITypeInfo ResolveEntityType(string entityTypeName, IMessagingContext context)
        {
            var entityType = this.typeResolver.ResolveType(entityTypeName, throwOnNotFound: false);
            if (entityType == null)
            {
                // TODO localization
                throw new InvalidOperationException($"Could not resolve type '{entityTypeName}'.");
            }

            return this.typeRegistry.GetTypeInfo(entityType);
        }

        /// <summary>
        /// Resolve the projected entity type.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="context">The messaging context.</param>
        /// <returns>
        /// An ITypeInfo.
        /// </returns>
        protected virtual ITypeInfo ResolveProjectedEntityType(ITypeInfo entityType, IMessagingContext context)
        {
            if (entityType is IRuntimeTypeInfo runtimeEntityType)
            {
                return this.typeRegistry.GetTypeInfo(this.projectedTypeResolver.ResolveProjectedType(runtimeEntityType.Type));
            }

            return entityType;
        }

        /// <summary>
        /// Resolve property information.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// An IPropertyInfo.
        /// </returns>
        protected virtual IPropertyInfo ResolveProperty(ITypeInfo entityType, string propertyName)
        {
            propertyName = propertyName.ToPascalCase();
            var propertyInfo = entityType.Properties.FirstOrDefault(p => p.Name == propertyName);
            if (propertyInfo == null)
            {
                // TODO localization
                throw new InvalidOperationException($"Could not resolve property '{propertyName}' in type '{entityType}'.");
            }

            return propertyInfo;
        }
    }
}