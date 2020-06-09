// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataImportService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default data import service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Import
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Data;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Conversion;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Internal;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Model;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default data import service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataImportService : Loggable, IDataImportService
    {
        private readonly IProjectedTypeResolver projectedTypeResolver;
        private readonly ICollection<IExportFactory<IDataImportBehavior, AppServiceMetadata>> behaviorFactories;
        private readonly IContextFactory contextFactory;
        private readonly IDataStreamReadService dataStreamReadService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataImportService"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="dataStreamReadService">The data source read service.</param>
        /// <param name="conversionService">The conversion service.</param>
        /// <param name="projectedTypeResolver">The projected type resolver.</param>
        /// <param name="behaviorFactories">Optional. The behavior factories (optional).</param>
        public DefaultDataImportService(
            IContextFactory contextFactory,
            IDataStreamReadService dataStreamReadService,
            IDataConversionService conversionService,
            IProjectedTypeResolver projectedTypeResolver,
            ICollection<IExportFactory<IDataImportBehavior, AppServiceMetadata>>? behaviorFactories = null)
            : base(contextFactory)
        {
            Requires.NotNull(contextFactory, nameof(contextFactory));
            Requires.NotNull(dataStreamReadService, nameof(dataStreamReadService));
            Requires.NotNull(conversionService, nameof(conversionService));
            Requires.NotNull(projectedTypeResolver, nameof(projectedTypeResolver));

            this.contextFactory = contextFactory;
            this.dataStreamReadService = dataStreamReadService;
            this.projectedTypeResolver = projectedTypeResolver;
            this.behaviorFactories = (behaviorFactories ?? new List<IExportFactory<IDataImportBehavior, AppServiceMetadata>>())
                                        .OrderBy(f => f.Metadata.ProcessingPriority)
                                        .ToList();
            this.ConversionService = conversionService;
        }

        /// <summary>
        /// Gets the conversion service.
        /// </summary>
        /// <value>
        /// The conversion service.
        /// </value>
        public IDataConversionService ConversionService { get; }

        /// <summary>
        /// Imports the data asynchronously.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token (optional).</param>
        /// <returns>
        /// A data import result.
        /// </returns>
        public IOperationResult ImportDataAsync(
            DataStream dataSource,
            Action<IDataImportContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            var context = this.CreateDataImportContext(optionsConfig);

            Requires.NotNull(context.DataSpace, nameof(context.DataSpace));

            var job = new DataSourceImportJob(dataSource, context, this.dataStreamReadService, this.ConversionService, this.projectedTypeResolver, this.behaviorFactories);
            return job.ExecuteAsync(cancellationToken);
        }

        /// <summary>
        /// Creates data import context.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The new data import context.
        /// </returns>
        protected virtual IDataImportContext CreateDataImportContext(Action<IDataImportContext>? optionsConfig = null)
        {
            var context = this.contextFactory.CreateContext<DataImportContext>();
            optionsConfig?.Invoke(context);
            return context;
        }

        /// <summary>
        /// The import job.
        /// </summary>
        internal class DataSourceImportJob : DataIOJobBase<IDataImportContext>
        {
            private readonly DataStream dataSource;
            private readonly IDataStreamReadService dataSourceReader;
            private readonly IDataSpace dataSpace;
            private readonly IDataConversionService conversionService;
            private readonly IProjectedTypeResolver projectedTypeResolver;
            private readonly ICollection<IExportFactory<IDataImportBehavior, AppServiceMetadata>> behaviorFactories;

            /// <summary>
            /// Initializes a new instance of the <see cref="DataSourceImportJob" /> class.
            /// </summary>
            /// <param name="dataSource">The data source.</param>
            /// <param name="context">The context.</param>
            /// <param name="dataSourceReader">The converter.</param>
            /// <param name="conversionService">The conversion service.</param>
            /// <param name="projectedTypeResolver">The projected type resolver.</param>
            /// <param name="behaviorFactories">The behavior factories.</param>
            public DataSourceImportJob(
                DataStream dataSource,
                IDataImportContext context,
                IDataStreamReadService dataSourceReader,
                IDataConversionService conversionService,
                IProjectedTypeResolver projectedTypeResolver,
                ICollection<IExportFactory<IDataImportBehavior, AppServiceMetadata>> behaviorFactories)
            : base(context)
            {
                this.dataSource = dataSource;
                this.dataSourceReader = dataSourceReader;
                this.dataSpace = context.DataSpace;
                this.conversionService = conversionService;
                this.projectedTypeResolver = projectedTypeResolver;
                this.behaviorFactories = behaviorFactories;
            }

            /// <summary>
            /// Executes the import job (core implementation).
            /// </summary>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>
            /// An operation result.
            /// </returns>
            protected override async Task<IOperationResult> ExecuteCoreAsync(CancellationToken cancellationToken)
            {
                var behaviors = this.behaviorFactories.Select(f => f.CreateExportedValue()).ToList();
                var reversedBehaviors = new List<IDataImportBehavior>(behaviors);
                reversedBehaviors.Reverse();

                cancellationToken.ThrowIfCancellationRequested();
                foreach (var behavior in behaviors)
                {
                    await behavior.BeforeReadDataSourceAsync(this.dataSource, this.Context, cancellationToken)
                        .PreserveThreadContext();
                }

                cancellationToken.ThrowIfCancellationRequested();
                var readResult = await this.dataSourceReader.ReadAsync(this.dataSource, this.Context, cancellationToken).PreserveThreadContext();
                if (!readResult.GetType().IsCollection())
                {
                    readResult = new List<object> { readResult };
                }

                var sourceEntities = ((IEnumerable<object>)readResult).ToList();

                cancellationToken.ThrowIfCancellationRequested();
                foreach (var behavior in reversedBehaviors)
                {
                    await behavior.AfterReadDataSourceAsync(this.dataSource, this.Context, sourceEntities, cancellationToken)
                        .PreserveThreadContext();
                }

                cancellationToken.ThrowIfCancellationRequested();
                await this.ImportAsManyAsPossibleAsync(sourceEntities, behaviors, reversedBehaviors, cancellationToken).PreserveThreadContext();

                return this.Result;
            }

            /// <summary>
            /// Imports as many as possible.
            /// </summary>
            /// <param name="sourceEntities">The source entities.</param>
            /// <param name="behaviors">The behaviors.</param>
            /// <param name="reversedBehaviors">The reversed behaviors.</param>
            /// <param name="cancellationToken">Optional. The cancellation token.</param>
            /// <returns>
            /// A task for continuation.
            /// </returns>
            private async Task ImportAsManyAsPossibleAsync(IEnumerable<object> sourceEntities, IList<IDataImportBehavior> behaviors, IList<IDataImportBehavior> reversedBehaviors, CancellationToken cancellationToken = default)
            {
                // ensure we run in a separate thread.
                await Task.Yield();

                var importEntityEntries = sourceEntities.Select(this.AttachSourceEntity).ToList();
                var percentageStep = 1f / importEntityEntries.Count;
                foreach (var importEntityEntry in importEntityEntries)
                {
                    IDataContext? targetDataContext = null;
                    try
                    {
                        // Convert the entity to import
                        cancellationToken.ThrowIfCancellationRequested();
                        foreach (var behavior in behaviors)
                        {
                            await behavior.BeforeConvertEntityAsync(importEntityEntry, this.Context, cancellationToken)
                                .PreserveThreadContext();
                        }

                        cancellationToken.ThrowIfCancellationRequested();
                        var targetEntry = await this.ConvertEntityAsync(importEntityEntry, cancellationToken).PreserveThreadContext();
                        targetDataContext = targetEntry.DataContext ?? this.dataSpace[targetEntry.Entity.GetType()];

                        // Persist the converted entity
                        cancellationToken.ThrowIfCancellationRequested();
                        foreach (var behavior in behaviors)
                        {
                            await behavior.BeforePersistEntityAsync(importEntityEntry, targetEntry, this.Context, cancellationToken)
                                .PreserveThreadContext();
                        }

                        cancellationToken.ThrowIfCancellationRequested();
                        var persistContext = new PersistChangesContext(targetDataContext);
                        this.Context.PersistChangesConfig?.Invoke(persistContext);
                        await targetDataContext.PersistChangesAsync(persistContext, cancellationToken).PreserveThreadContext();

                        importEntityEntry.AcceptChanges();
                        this.Result.MergeMessage(new ImportEntitySuccessfulMessage(importEntityEntry.Entity));

                        // The converted entity is persisted
                        cancellationToken.ThrowIfCancellationRequested();
                        foreach (var behavior in reversedBehaviors)
                        {
                            await behavior.AfterPersistEntityAsync(importEntityEntry, targetEntry, this.Context, cancellationToken)
                                .PreserveThreadContext();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        targetDataContext?.DiscardChanges();
                        throw;
                    }
                    catch (Exception ex)
                    {
                        this.Result.MergeException(new ImportEntityException(importEntityEntry.Entity, ex));
                        targetDataContext?.DiscardChanges();
                    }

                    this.Result.PercentCompleted += percentageStep;
                }
            }

            /// <summary>
            /// Gets the entity entry based on a source object.
            /// </summary>
            /// <param name="source">The source object.</param>
            /// <returns>
            /// The entity entry.
            /// </returns>
            private IEntityEntry AttachSourceEntity(object source)
            {
                object entity;
                ChangeState changeState;
                if (source is IChangeStateTrackableEntityEntry changeStateTrackableEntityEntry)
                {
                    // the imported entity is already an IEntityEntry wrapper. Adjust only the change state.
                    entity = changeStateTrackableEntityEntry.Entity;
                    changeState = changeStateTrackableEntityEntry.ChangeState;
                    if (changeState == ChangeState.NotChanged)
                    {
                        changeState = ChangeState.AddedOrChanged;
                    }
                }
                else
                {
                    // the imported entity is the real entity. Set the change state as added or changed.
                    entity = source;
                    changeState = ChangeState.AddedOrChanged;
                }

                var sourceDataContext = this.dataSpace[entity.GetType()];
                var sourceEntityEntry = sourceDataContext.Attach(entity);
                sourceEntityEntry.ChangeState = changeState;

                return sourceEntityEntry;
            }

            /// <summary>
            /// Converts the entity.
            /// </summary>
            /// <param name="sourceEntityEntry">The source entity info.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>
            /// A promise of the imported target entity.
            /// </returns>
            private async Task<IEntityEntry> ConvertEntityAsync(IEntityEntry sourceEntityEntry, CancellationToken cancellationToken = default)
            {
                var sourceEntity = sourceEntityEntry.Entity;
                using var conversionContext = new DataConversionContext(this.dataSpace, this.conversionService)
                    .RootTargetType(this.projectedTypeResolver.ResolveProjectedType(sourceEntity.GetType(), this.Context));
                this.Context.DataConversionConfig?.Invoke(sourceEntity, conversionContext);

                cancellationToken.ThrowIfCancellationRequested();

                var conversionResult = await this.conversionService.ConvertAsync(sourceEntity, (object?)null, conversionContext, cancellationToken).PreserveThreadContext();

                cancellationToken.ThrowIfCancellationRequested();

                var targetEntity = conversionResult.Target;
                var targetDataContext = this.dataSpace[targetEntity.GetType()];
                var targetEntityEntry = targetDataContext.Attach(targetEntity);

                // force the change of the entity change state only if
                // * the target entity is not changed - or -
                // * the source entity indicated deletion.
                if (targetEntityEntry.ChangeState == ChangeState.NotChanged
                    || sourceEntityEntry.ChangeState == ChangeState.Deleted)
                {
                    targetEntityEntry.ChangeState = sourceEntityEntry.ChangeState;
                }

                return targetEntityEntry;
            }
        }
    }
}