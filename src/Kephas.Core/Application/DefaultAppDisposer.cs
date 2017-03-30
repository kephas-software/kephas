// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppDisposer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default application disposer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Diagnostics;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Resources;
    using Kephas.Services;
    using Kephas.Services.Behavior;
    using Kephas.Services.Composition;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default application disposer.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultAppDisposer : IAppDisposer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAppDisposer"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="compositionContext">Context for the composition.</param>
        /// <param name="appFinalizerFactories">The app finalizer factories.</param>
        /// <param name="appFinalizerBehaviorFactories">The application finalizer behavior factories.</param>
        public DefaultAppDisposer(
            IAmbientServices ambientServices, 
            ICompositionContext compositionContext, 
            ICollection<IExportFactory<IAppFinalizer, AppServiceMetadata>> appFinalizerFactories,
            ICollection<IExportFactory<IAppFinalizerBehavior, AppServiceMetadata>> appFinalizerBehaviorFactories)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(compositionContext, nameof(compositionContext));

            this.AmbientServices = ambientServices;
            this.CompositionContext = compositionContext;
            this.AppFinalizerFactories = appFinalizerFactories ?? new List<IExportFactory<IAppFinalizer, AppServiceMetadata>>();
            this.AppFinalizerBehaviorFactories = appFinalizerBehaviorFactories ?? new List<IExportFactory<IAppFinalizerBehavior, AppServiceMetadata>>();
        }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public IAmbientServices AmbientServices { get; }

        /// <summary>
        /// Gets a context for the composition.
        /// </summary>
        /// <value>
        /// The composition context.
        /// </value>
        public ICompositionContext CompositionContext { get; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<IAppBootstrapper> Logger { get; set; }

        /// <summary>
        /// Gets the application finalizer factories.
        /// </summary>
        /// <value>
        /// The application finalizer factories.
        /// </value>
        public ICollection<IExportFactory<IAppFinalizer, AppServiceMetadata>> AppFinalizerFactories { get; }

        /// <summary>
        /// Gets the application finalizer behavior factories.
        /// </summary>
        /// <value>
        /// The application finalizer behavior factories.
        /// </value>
        public ICollection<IExportFactory<IAppFinalizerBehavior, AppServiceMetadata>> AppFinalizerBehaviorFactories { get; }

        /// <summary>
        /// Disposes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public virtual async Task DisposeAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await Profiler.WithInfoStopwatchAsync(
                    async () =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        await this.BeforeDisposeAsync(appContext, cancellationToken).PreserveThreadContext();
                        cancellationToken.ThrowIfCancellationRequested();

                        await this.RunFinalizersAsync(appContext, cancellationToken).PreserveThreadContext();
                        cancellationToken.ThrowIfCancellationRequested();

                        await this.AfterDisposeAsync(appContext, cancellationToken).PreserveThreadContext();
                        cancellationToken.ThrowIfCancellationRequested();
                    },
                    this.Logger).PreserveThreadContext();
            }
            catch (OperationCanceledException)
            {
                this.Logger.Error(Strings.DefaultAppDisposer_DisposeCanceled_Exception, DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, Strings.DefaultAppDisposer_DisposeFaulted_Exception, DateTimeOffset.Now);
            }
        }

        /// <summary>
        /// Overridable method called before actually disposing the application.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task BeforeDisposeAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            return TaskHelper.CompletedTask;
        }

        /// <summary>
        /// Overridable method called after the application was disposed.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task AfterDisposeAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            return TaskHelper.CompletedTask;
        }

        /// <summary>
        /// Executes the finalizers asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual async Task RunFinalizersAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            var orderedAppFinalizerExports = this.AppFinalizerFactories
                                          .Select(factory => factory.CreateExport())
                                          .WhereEnabled(this.AmbientServices)
                                          .OrderBy(export => export.Metadata.ProcessingPriority)
                                          .ToList();

            var orderedBehaviors = this.AppFinalizerBehaviorFactories
                                          .Select(factory => factory.CreateExport())
                                          .WhereEnabled(this.AmbientServices)
                                          .OrderBy(export => export.Metadata.ProcessingPriority)
                                          .ToList();

            var reverseOrderedBehaviors = orderedBehaviors
                                          .OrderByDescending(export => export.Metadata.ProcessingPriority)
                                          .ToList();

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var appFinalizerFactory in orderedAppFinalizerExports)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var appFinalizer = appFinalizerFactory.Value;
                var appFinalizerMetadata = appFinalizerFactory.Metadata;

                var appFinalizerType = appFinalizer.GetType();
                var appFinalizerIdentifier = $"AppFinalizer '{appFinalizerType}' (#{appFinalizerMetadata.ProcessingPriority})";
                try
                {
                    await Profiler.WithInfoStopwatchAsync(
                        async () =>
                            {
                                await this.RunBeforeFinalizerBehaviorsAsync(orderedBehaviors, appContext, appFinalizerMetadata, cancellationToken).PreserveThreadContext();
                                await appFinalizer.FinalizeAsync(appContext, cancellationToken).PreserveThreadContext();
                                await this.RunAfterFinalizerBehaviorsAsync(reverseOrderedBehaviors, appContext, appFinalizerMetadata, cancellationToken).PreserveThreadContext();
                            },
                        this.Logger,
                        appFinalizerIdentifier).PreserveThreadContext();
                }
                catch (OperationCanceledException cex)
                {
                    this.Logger.Error(cex, $"{appFinalizerIdentifier} was canceled during disposal. The current operation will be interrupted.");
                    throw;
                }
                catch (Exception ex)
                {
                    var initializerKind = appFinalizerMetadata.OptionalService ? "optional" : "required";
                    this.Logger.Error(ex, $"{appFinalizerIdentifier} ({initializerKind}) failed to dispose. See the inner exception for more details.");

                    // do not interrupt the disposal if a required finalizer failed to finalize.
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        /// <summary>
        /// Executes the before finalization behaviors.
        /// </summary>
        /// <param name="behaviors">The behaviors.</param>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="appServiceMetadata">The application service metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual async Task RunBeforeFinalizerBehaviorsAsync(
            ICollection<IExport<IAppFinalizerBehavior, AppServiceMetadata>> behaviors,
            IAppContext appContext,
            AppServiceMetadata appServiceMetadata,
            CancellationToken cancellationToken)
        {
            foreach (var behavior in behaviors)
            {
                await behavior.Value.BeforeFinalizeAsync(appContext, appServiceMetadata, cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Executes the after finalization behaviors.
        /// </summary>
        /// <param name="behaviors">The behaviors.</param>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="appServiceMetadata">The application service metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual async Task RunAfterFinalizerBehaviorsAsync(
            ICollection<IExport<IAppFinalizerBehavior, AppServiceMetadata>> behaviors,
            IAppContext appContext,
            AppServiceMetadata appServiceMetadata,
            CancellationToken cancellationToken)
        {
            foreach (var behavior in behaviors)
            {
                await behavior.Value.AfterFinalizeAsync(appContext, appServiceMetadata, cancellationToken).PreserveThreadContext();
            }
        }
    }
}