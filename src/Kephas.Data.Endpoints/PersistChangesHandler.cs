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

    using Kephas.Data.Capabilities;
    using Kephas.Data.Client.Capabilities;
    using Kephas.Data.Conversion;
    using Kephas.Services;
    using Kephas.Messaging;
    using Kephas.Model;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A persist changes handler.
    /// </summary>
    public class PersistChangesHandler : IMessageHandler<PersistChangesMessage, PersistChangesResponse>
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
            dataConversionService = dataConversionService ?? throw new System.ArgumentNullException(nameof(dataConversionService));
            projectedTypeResolver = projectedTypeResolver ?? throw new System.ArgumentNullException(nameof(projectedTypeResolver));
            dataSpaceFactory = dataSpaceFactory ?? throw new System.ArgumentNullException(nameof(dataSpaceFactory));

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
        public async Task<PersistChangesResponse> ProcessAsync(PersistChangesMessage message, IMessagingContext context, CancellationToken token)
        {
            var mappings = new List<(DtoEntityEntry dtoEntry, object entity)>();
            var response = new PersistChangesResponse();

            if (message.EntityEntries == null || message.EntityEntries.Count == 0)
            {
                return response;
            }

            var dataSpaceContext = new Context(context).InitialData(message.EntityEntries);
            using (var dataSpace = this.dataSpaceFactory.CreateInitializedValue(dataSpaceContext))
            {
                // convert to entities
                foreach (var dtoEntityEntry in message.EntityEntries.Where(e => e.Entity != null))
                {
                    // gets the domain entity and sets the values from the client
                    var dtoEntity = dtoEntityEntry.Entity;
                    var dtoEntityType = dtoEntity.GetType();
                    var domainEntityType = this.projectedTypeResolver.ResolveProjectedType(dtoEntityType);

                    if (dtoEntityType != domainEntityType)
                    {
                        using (var conversionContext = new DataConversionContext(dataSpace, this.dataConversionService)
                                    .RootTargetType(domainEntityType))
                        {
                            var result = await this.dataConversionService.ConvertAsync(dtoEntity, (object)null, conversionContext, token).PreserveThreadContext();
                            mappings.Add((dtoEntry: dtoEntityEntry, result.Target));

                            // deleted entities are marked as deleted
                            if (dtoEntityEntry.ChangeState == ChangeState.Deleted)
                            {
                                var domainDataContext = dataSpace[domainEntityType];
                                var changeStateEntity = domainDataContext.GetEntityEntry(result.Target);
                                changeStateEntity.ChangeState = ChangeState.Deleted;
                            }
                        }
                    }
                    else
                    {
                        mappings.Add((dtoEntry: dtoEntityEntry, clientEntity: dtoEntity));
                    }

                    // add a response entry in the response
                    var originalId = (dtoEntity as IIdentifiable)?.Id;
                    response.EntityEntries.Add(new DtoEntityEntry
                    {
                        ChangeState = dtoEntityEntry.ChangeState,
                        Entity = dtoEntityEntry.ChangeState == ChangeState.Deleted ? null : dtoEntity,
                        OriginalEntityId = originalId,
                        EntityTypeName = this.GetClientTypeName(dtoEntity),
                    });
                }

                // prepare the persistence
                await this.PrePersistChangesAsync(response, mappings, dataSpace, cancellationToken: token).PreserveThreadContext();

                // save changes
                await dataSpace.PersistChangesAsync(cancellationToken: token).PreserveThreadContext();

                // finalize the persistence
                await this.PostPersistChangesAsync(response, mappings, dataSpace, cancellationToken: token).PreserveThreadContext();
            }

            foreach (var entry in response.EntityEntries)
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
            PersistChangesResponse response,
            IList<(DtoEntityEntry clientEntry, object entity)> mappings,
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
            PersistChangesResponse response,
            IList<(DtoEntityEntry dtoEntityEntry, object entity)> mappings,
            IDataSpace dataSpace,
            CancellationToken cancellationToken)
        {
            // convert back to client entities
            foreach (var (dtoEntityEntry, entity) in mappings)
            {
                if (dtoEntityEntry.ChangeState != ChangeState.Deleted && dtoEntityEntry.Entity != entity)
                {
                    var conversionContext = this.CreateDataConversionContextForResponse(dataSpace);
                    var result = await this.dataConversionService.ConvertAsync(entity, dtoEntityEntry.Entity, conversionContext, cancellationToken).PreserveThreadContext();
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
            var conversionContext = new DataConversionContext(dataSpace, this.dataConversionService);
            return conversionContext;
        }
    }
}