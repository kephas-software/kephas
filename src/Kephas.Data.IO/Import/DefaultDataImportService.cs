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
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Data;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Conversion;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Diagnostics;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Model.Services;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default data import service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataImportService : IDataImportService
    {
        /// <summary>
        /// The projected type resolver.
        /// </summary>
        private readonly IProjectedTypeResolver projectedTypeResolver;

        /// <summary>
        /// The behavior behaviorFactories.
        /// </summary>
        private readonly ICollection<IExportFactory<IDataImportBehavior, AppServiceMetadata>> behaviorFactories;

        /// <summary>
        /// The data source reader provider.
        /// </summary>
        private readonly IDataStreamReadService dataStreamReadService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataImportService"/> class.
        /// </summary>
        /// <param name="dataStreamReadService">The data source read service.</param>
        /// <param name="conversionService">The conversion service.</param>
        /// <param name="projectedTypeResolver">The projected type resolver.</param>
        /// <param name="behaviorFactories">The behavior factories (optional).</param>
        public DefaultDataImportService(
            IDataStreamReadService dataStreamReadService,
            IDataConversionService conversionService,
            IProjectedTypeResolver projectedTypeResolver,
            ICollection<IExportFactory<IDataImportBehavior, AppServiceMetadata>> behaviorFactories = null)
        {
            Requires.NotNull(dataStreamReadService, nameof(dataStreamReadService));

            this.dataStreamReadService = dataStreamReadService;
            this.projectedTypeResolver = projectedTypeResolver;
            this.behaviorFactories = (behaviorFactories ?? new List<IExportFactory<IDataImportBehavior, AppServiceMetadata>>())
                                        .OrderBy(f => f.Metadata.ProcessingPriority)
                                        .ToList();
            this.ConversionService = conversionService;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<DefaultDataImportService> Logger { get; set; }

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
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A data import result.
        /// </returns>
        public async Task<IOperationResult> ImportDataAsync(
            DataStream dataSource,
            IDataImportContext context,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(context.DataSpace, nameof(context.DataSpace));

            var result = context.EnsureResult();
            result.OperationState = OperationState.InProgress;

            IOperationResult jobResult = null;
            var elapsed = await Profiler.WithStopwatchAsync(
                async () =>
                    {
                        var job = this.CreateImportJob(dataSource, context, result);
                        jobResult = await job.ExecuteAsync(cancellationToken).PreserveThreadContext();
                    }).PreserveThreadContext();

            result.MergeResult(jobResult);
            result.Elapsed = elapsed;
            result.OperationState = OperationState.Completed;
            return result;
        }

        /// <summary>
        /// Creates the import job.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <returns>
        /// A list of import jobs.
        /// </returns>
        private DataSourceImportJob CreateImportJob(DataStream dataSource, IDataImportContext context, IOperationResult result)
        {
            var job = new DataSourceImportJob(dataSource, context, this.dataStreamReadService, this.ConversionService, this.projectedTypeResolver, this.behaviorFactories);
            job.PropertyChanged += (j, ea) => result.PercentCompleted = ((DataSourceImportJob)j).PercentCompleted;

            return job;
        }

        /// <summary>
        /// The import job.
        /// </summary>
        internal class DataSourceImportJob : INotifyPropertyChanged, IDisposable
        {
            /// <summary>
            /// The data source.
            /// </summary>
            private readonly DataStream dataSource;

            /// <summary>
            /// The context.
            /// </summary>
            private readonly IDataImportContext context;

            /// <summary>
            /// The converter.
            /// </summary>
            private readonly IDataStreamReadService dataSourceReader;

            /// <summary>
            /// The data space.
            /// </summary>
            private readonly IDataSpace dataSpace;

            /// <summary>
            /// The conversion service.
            /// </summary>
            private readonly IDataConversionService conversionService;

            /// <summary>
            /// The projected type resolver.
            /// </summary>
            private readonly IProjectedTypeResolver projectedTypeResolver;

            /// <summary>
            /// The behavior factories.
            /// </summary>
            private readonly ICollection<IExportFactory<IDataImportBehavior, AppServiceMetadata>> behaviorFactories;

            /// <summary>
            /// The percent completed.
            /// </summary>
            private float percentCompleted;

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
            {
                this.dataSource = dataSource;
                this.context = context;
                this.dataSourceReader = dataSourceReader;
                this.dataSpace = context.DataSpace;
                this.conversionService = conversionService;
                this.projectedTypeResolver = projectedTypeResolver;
                this.behaviorFactories = behaviorFactories;
            }

            /// <summary>
            /// Occurs when a property value changes.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets or sets the percent completed.
            /// </summary>
            /// <value>
            /// The percent completed.
            /// </value>
            public float PercentCompleted
            {
                get => this.percentCompleted;
                set => this.SetProperty(ref this.percentCompleted, value);
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
            }

            /// <summary>
            /// Executes the import job.
            /// </summary>
            /// <param name="cancellationToken">The cancellation token (optional).</param>
            /// <returns>
            /// A data exchange result.
            /// </returns>
            public async Task<IOperationResult> ExecuteAsync(CancellationToken cancellationToken = default)
            {
                var result = new OperationResult();

                var behaviors = this.behaviorFactories.Select(f => f.CreateExportedValue()).ToList();
                var reversedBehaviors = new List<IDataImportBehavior>(behaviors);
                reversedBehaviors.Reverse();

                cancellationToken.ThrowIfCancellationRequested();
                foreach (var behavior in behaviors)
                {
                    await behavior.BeforeReadDataSourceAsync(this.dataSource, this.context, cancellationToken)
                        .PreserveThreadContext();
                }

                cancellationToken.ThrowIfCancellationRequested();
                var readResult = await this.dataSourceReader.ReadAsync(this.dataSource, this.context, cancellationToken).PreserveThreadContext();
                if (!readResult.GetType().IsCollection())
                {
                    readResult = new List<object> { readResult };
                }

                var sourceEntities = ((IEnumerable<object>)readResult).ToList();

                cancellationToken.ThrowIfCancellationRequested();
                foreach (var behavior in reversedBehaviors)
                {
                    await behavior.AfterReadDataSourceAsync(this.dataSource, this.context, sourceEntities, cancellationToken)
                        .PreserveThreadContext();
                }

                cancellationToken.ThrowIfCancellationRequested();
                await this.ImportAsManyAsPossibleAsync(sourceEntities, result, behaviors, reversedBehaviors, cancellationToken).PreserveThreadContext();

                return result;
            }

            /// <summary>
            /// Called when a property changes.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            protected virtual void OnPropertyChanged(string propertyName = null)
            {
                var handler = this.PropertyChanged;
                handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            /// <summary>
            /// Sets the property.
            /// </summary>
            /// <typeparam name="T">The field type.</typeparam>
            /// <param name="field">The field.</param>
            /// <param name="value">The value.</param>
            /// <param name="name">The name.</param>
            private void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
            {
                if (EqualityComparer<T>.Default.Equals(field, value))
                {
                    return;
                }

                field = value;
                this.OnPropertyChanged(name);
            }

            /// <summary>
            /// Imports as many as possible.
            /// </summary>
            /// <param name="sourceEntities">The source entities.</param>
            /// <param name="result">The result.</param>
            /// <param name="behaviors">The behaviors.</param>
            /// <param name="reversedBehaviors">The reversed behaviors.</param>
            /// <param name="cancellationToken">Optional. The cancellation token.</param>
            /// <returns>
            /// A task for continuation.
            /// </returns>
            private async Task ImportAsManyAsPossibleAsync(IEnumerable<object> sourceEntities, IOperationResult result, IList<IDataImportBehavior> behaviors, IList<IDataImportBehavior> reversedBehaviors, CancellationToken cancellationToken = default)
            {
                var importEntityEntries = sourceEntities.Select(this.AttachSourceEntity).ToList();
                foreach (var importEntityEntry in importEntityEntries)
                {
                    IDataContext targetDataContext = null;
                    try
                    {
                        // Convert the entity to import
                        cancellationToken.ThrowIfCancellationRequested();
                        foreach (var behavior in behaviors)
                        {
                            await behavior.BeforeConvertEntityAsync(importEntityEntry, this.context, cancellationToken)
                                .PreserveThreadContext();
                        }

                        cancellationToken.ThrowIfCancellationRequested();
                        var targetEntry = await this.ConvertEntityAsync(importEntityEntry, cancellationToken).PreserveThreadContext();
                        targetDataContext = targetEntry.DataContext ?? this.dataSpace[targetEntry.Entity.GetType()];

                        // Persist the converted entity
                        cancellationToken.ThrowIfCancellationRequested();
                        foreach (var behavior in behaviors)
                        {
                            await behavior.BeforePersistEntityAsync(importEntityEntry, targetEntry, this.context, cancellationToken)
                                .PreserveThreadContext();
                        }

                        cancellationToken.ThrowIfCancellationRequested();
                        var persistContext = new PersistChangesContext(targetDataContext);
                        this.context.PersistChangesContextConfig?.Invoke(persistContext);
                        await targetDataContext.PersistChangesAsync(persistContext, cancellationToken).PreserveThreadContext();

                        importEntityEntry.AcceptChanges();
                        result.MergeMessage(new ImportEntitySuccessfulMessage(importEntityEntry.Entity));

                        // The converted entity is persisted
                        cancellationToken.ThrowIfCancellationRequested();
                        foreach (var behavior in reversedBehaviors)
                        {
                            await behavior.AfterPersistEntityAsync(importEntityEntry, targetEntry, this.context, cancellationToken)
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
                        result.MergeException(new ImportEntityException(importEntityEntry.Entity, ex));
                        targetDataContext?.DiscardChanges();
                    }
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

                var sourceDataContext = this.dataSpace[entity.GetType(), this.context];
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
                var conversionContext = new DataConversionContextBuilder(this.dataSpace, this.conversionService)
                    .WithRootTargetType(this.projectedTypeResolver.ResolveProjectedType(sourceEntity.GetType(), this.context))
                    .ConversionContext;

                this.context.DataConversionContextConfig?.Invoke(sourceEntity, conversionContext);

                cancellationToken.ThrowIfCancellationRequested();

                var conversionResult = await this.conversionService.ConvertAsync(sourceEntity, (object)null, conversionContext, cancellationToken).PreserveThreadContext();

                cancellationToken.ThrowIfCancellationRequested();

                var targetEntity = conversionResult.Target;
                var targetDataContext = this.dataSpace[targetEntity.GetType(), this.context];
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