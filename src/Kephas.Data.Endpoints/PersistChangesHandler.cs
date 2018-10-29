// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistChangesHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the persist changes handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Endpoints
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Client.Capabilities;
    using Kephas.Data.Conversion;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging;
    using Kephas.Model.Services;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A persist changes handler.
    /// </summary>
    public class PersistChangesHandler : MessageHandlerBase<PersistChangesMessage, PersistChangesResponseMessage>
    {
        /// <summary>
        /// The data conversion service.
        /// </summary>
        private readonly IDataConversionService dataConversionService;

        /// <summary>
        /// The projected type resolver.
        /// </summary>
        private readonly IProjectedTypeResolver projectedTypeResolver;

        /// <summary>
        /// The data space factory.
        /// </summary>
        private readonly IExportFactory<IDataSpace> dataSpaceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistChangesHandler"/> class.
        /// </summary>
        /// <param name="dataConversionService">The data conversion service.</param>
        /// <param name="projectedTypeResolver">The projected type resolver.</param>
        /// <param name="dataSpaceFactory">The data space factory.</param>
        public PersistChangesHandler(
            IDataConversionService dataConversionService,
            IProjectedTypeResolver projectedTypeResolver,
            IExportFactory<IDataSpace> dataSpaceFactory)
        {
            Requires.NotNull(dataConversionService, nameof(dataConversionService));
            Requires.NotNull(projectedTypeResolver, nameof(projectedTypeResolver));
            Requires.NotNull(dataSpaceFactory, nameof(dataSpaceFactory));

            this.dataConversionService = dataConversionService;
            this.projectedTypeResolver = projectedTypeResolver;
            this.dataSpaceFactory = dataSpaceFactory;
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
        public override async Task<PersistChangesResponseMessage> ProcessAsync(PersistChangesMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            var mappings = new List<(ClientEntityInfo clientEntry, object entity)>();
            var response = new PersistChangesResponseMessage();

            if (message.EntityInfos == null || message.EntityInfos.Count == 0)
            {
                return response;
            }

            var dataSpaceContext = new Context(context).WithInitialData(message.EntityInfos);
            using (var dataSpace = this.dataSpaceFactory.CreateExportedValue(dataSpaceContext))
            {
                // convert to entities
                foreach (var clientEntityInfo in message.EntityInfos.Where(e => e.Entity != null))
                {
                    // gets the domain entity and sets the values from the client
                    var clientEntity = clientEntityInfo.Entity;
                    var clientEntityType = clientEntity.GetType();
                    var domainEntityType = this.projectedTypeResolver.ResolveProjectedType(clientEntityType);

                    if (clientEntityType != domainEntityType)
                    {
                        var conversionContext = new DataConversionContext(this.dataConversionService, dataSpace, rootTargetType: domainEntityType);
                        var result = await this.dataConversionService.ConvertAsync(clientEntity, (object)null, conversionContext, token).PreserveThreadContext();
                        mappings.Add((clientEntityInfo, result.Target));

                        // deleted entities are marked as deleted
                        if (clientEntityInfo.ChangeState == ChangeState.Deleted)
                        {
                            var domainDataContext = dataSpace[domainEntityType, context];
                            var changeStateEntity = domainDataContext.GetEntityInfo(result.Target);
                            changeStateEntity.ChangeState = ChangeState.Deleted;
                        }
                    }
                    else
                    {
                        mappings.Add((clientEntityInfo, clientEntity));
                    }

                    // add a response entry in the response
                    var originalId = (clientEntity as IIdentifiable)?.Id;
                    response.EntityInfos.Add(new ClientEntityInfo
                    {
                        ChangeState = clientEntityInfo.ChangeState,
                        Entity = clientEntityInfo.ChangeState == ChangeState.Deleted ? null : clientEntity,
                        OriginalEntityId = originalId,
                        EntityTypeName = this.GetClientTypeName(clientEntity),
                    });
                }

                // prepare the persistence
                await this.PrePersistChangesAsync(response, mappings, dataSpace, cancellationToken: token).PreserveThreadContext();

                // save changes
                foreach (var dataContext in dataSpace)
                {
                    await dataContext.PersistChangesAsync(cancellationToken: token).PreserveThreadContext();
                }

                // finalize the persistence
                await this.PostPersistChangesAsync(response, mappings, dataSpace, cancellationToken: token).PreserveThreadContext();
            }

            foreach (var entry in response.EntityInfos)
            {
                entry.ChangeState = entry.ChangeState == ChangeState.Deleted ? ChangeState.Deleted : ChangeState.NotChanged;
            }

            return response;
        }

        /// <summary>
        /// Pre persist changes asynchronously.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="mappings">The mappings.</param>
        /// <param name="dataSpace">Manager for data context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task PrePersistChangesAsync(
            PersistChangesResponseMessage response,
            IList<(ClientEntityInfo clientEntry, object entity)> mappings,
            IDataSpace dataSpace,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Post persist changes asynchronously.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="mappings">The mappings.</param>
        /// <param name="dataSpace">Manager for data context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual async Task PostPersistChangesAsync(
            PersistChangesResponseMessage response,
            IList<(ClientEntityInfo clientEntry, object entity)> mappings,
            IDataSpace dataSpace,
            CancellationToken cancellationToken)
        {
            // convert back to client entities
            foreach (var (clientEntityInfo, entity) in mappings)
            {
                if (clientEntityInfo.ChangeState != ChangeState.Deleted && clientEntityInfo.Entity != entity)
                {
                    var conversionContext = this.CreateDataConversionContextForResponse(dataSpace);
                    var result = await this.dataConversionService.ConvertAsync(entity, clientEntityInfo.Entity, conversionContext, cancellationToken).PreserveThreadContext();
                }
            }
        }

        /// <summary>
        /// Gets the type name of the client entity.
        /// </summary>
        /// <param name="clientEntity">The client entity.</param>
        /// <returns>
        /// The deleted entity.
        /// </returns>
        protected virtual string GetClientTypeName(object clientEntity)
        {
            return clientEntity.GetType().FullName;
        }

        /// <summary>
        /// Creates data conversion context for response.
        /// </summary>
        /// <param name="dataSpace">Manager for data context.</param>
        /// <returns>
        /// The new data conversion context for response.
        /// </returns>
        protected virtual IDataConversionContext CreateDataConversionContextForResponse(IDataSpace dataSpace)
        {
            var conversionContext = new DataConversionContext(this.dataConversionService, dataSpace);
            return conversionContext;
        }
    }
}