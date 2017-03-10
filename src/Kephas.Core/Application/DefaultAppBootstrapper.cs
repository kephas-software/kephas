// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppBootstrapper.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default application bootstrapper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
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
    /// A default application bootstrapper.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultAppBootstrapper : IAppBootstrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAppBootstrapper"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="compositionContext">Context for the composition.</param>
        /// <param name="appIntializerFactories">The app intializer factories.</param>
        public DefaultAppBootstrapper(IAmbientServices ambientServices, ICompositionContext compositionContext, ICollection<IExportFactory<IAppInitializer, AppServiceMetadata>> appIntializerFactories)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(compositionContext, nameof(compositionContext));

            this.AmbientServices = ambientServices;
            this.CompositionContext = compositionContext;
            this.AppIntializerFactories = appIntializerFactories ?? new List<IExportFactory<IAppInitializer, AppServiceMetadata>>();
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
        /// Gets the application intializer factories.
        /// </summary>
        /// <value>
        /// The application intializer factories.
        /// </value>
        public ICollection<IExportFactory<IAppInitializer, AppServiceMetadata>> AppIntializerFactories { get; }

        /// <summary>
        /// Starts the application asynchronously.
        /// </summary>
        /// <param name="appContext">       Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public virtual async Task StartAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await Profiler.WithInfoStopwatchAsync(
                    async () =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await this.BeforeStartAsync(appContext, cancellationToken).PreserveThreadContext();
                            cancellationToken.ThrowIfCancellationRequested();

                            await this.RunInitializersAsync(appContext, cancellationToken).PreserveThreadContext();
                            cancellationToken.ThrowIfCancellationRequested();

                            await this.AfterStartAsync(appContext, cancellationToken).PreserveThreadContext();
                            cancellationToken.ThrowIfCancellationRequested();
                        },
                    this.Logger).PreserveThreadContext();
            }
            catch (OperationCanceledException)
            {
                this.Logger.Error(Strings.DefaultAppBootstrapper_StartCanceled_Exception, DateTimeOffset.Now);
                throw;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, Strings.DefaultAppBootstrapper_StartFaulted_Exception, DateTimeOffset.Now);
                throw;
            }
        }

        /// <summary>
        /// Overridable method called before actually starting the application.
        /// </summary>
        /// <param name="appContext">       Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task BeforeStartAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            return TaskHelper.CompletedTask;
        }

        /// <summary>
        /// Overridable method called after the application started.
        /// </summary>
        /// <param name="appContext">       Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task AfterStartAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            return TaskHelper.CompletedTask;
        }

        /// <summary>
        /// Executes the initializers asynchronously.
        /// </summary>
        /// <param name="appContext">       Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual async Task RunInitializersAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            var orderedAppInitializerExports = this.AppIntializerFactories
                                          .Select(factory => factory.CreateExport())
                                          .WhereEnabled(this.AmbientServices)
                                          .OrderBy(export => export.Metadata.ProcessingPriority)
                                          .ToList();

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var appInitializerFactory in orderedAppInitializerExports)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var appInitializer = appInitializerFactory.Value;
                var appInitializerMetadata = appInitializerFactory.Metadata;

                var appInitializerType = appInitializer.GetType();
                var appInitializerIdentifier = $"AppInitializer '{appInitializerType}' (#{appInitializerMetadata.ProcessingPriority})";
                try
                {
                    await Profiler.WithInfoStopwatchAsync(
                        () => appInitializer.InitializeAsync(appContext, cancellationToken),
                        this.Logger,
                        appInitializerIdentifier).PreserveThreadContext();
                }
                catch (OperationCanceledException cex)
                {
                    this.Logger.Error(cex, $"{appInitializerIdentifier} was canceled during initialization. The current operation will be interrupted.");
                    throw;
                }
                catch (Exception ex)
                {
                    var initializerKind = appInitializerMetadata.OptionalService ? "optional" : "required";
                    this.Logger.Error(ex, $"{appInitializerIdentifier} ({initializerKind}) failed to initialize. See the inner exception for more details.");

                    // interrupt the bootstrapping if a required initializer failed to start.
                    if (!appInitializerMetadata.OptionalService)
                    {
                        throw;
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}