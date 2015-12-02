namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Diagnostics;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default application boostrapper.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultAppBoostrapper : IAppBootstrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAppBoostrapper"/> class.
        /// </summary>
        /// <param name="appIntializerFactories">The app intializer factories.</param>
        public DefaultAppBoostrapper(ICollection<IExportFactory<IAppInitializer, AppServiceMetadata>> appIntializerFactories)
        {
            this.AppIntializerFactories = appIntializerFactories ?? new List<IExportFactory<IAppInitializer, AppServiceMetadata>>();
        }

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

                            await this.BeforeStartAsync(appContext, cancellationToken).WithServerThreadingContext();
                            cancellationToken.ThrowIfCancellationRequested();

                            await this.RunInitializersAsync(appContext, cancellationToken).WithServerThreadingContext();
                            cancellationToken.ThrowIfCancellationRequested();

                            await this.AfterStartAsync(appContext, cancellationToken).WithServerThreadingContext();
                            cancellationToken.ThrowIfCancellationRequested();
                        },
                    this.Logger).WithServerThreadingContext();
            }
            catch (OperationCanceledException)
            {
                this.Logger.Error($"The boostrapper start procedure was canceled, at {DateTimeOffset.Now:s}.");
                throw;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, $"The boostrapper encountered an exception while starting, at {DateTimeOffset.Now:s}.");
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
            return CompletedTask.Value;
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
            return CompletedTask.Value;
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
            var appInitializers = this.AppIntializerFactories
                                          .OrderBy(i => i.Metadata.ProcessingPriority)
                                          .Select(i => Tuple.Create(i.CreateExport().Value, i.Metadata))
                                          .ToList();

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var appInitializer in appInitializers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var initializerType = appInitializer.GetType();
                var appInitializerIdentifier = $"AppInitializer '{initializerType}'";
                try
                {
                    await Profiler.WithInfoStopwatchAsync(
                        () => appInitializer.Item1.InitializeAsync(appContext, cancellationToken),
                        this.Logger,
                        appInitializerIdentifier).WithServerThreadingContext();
                }
                catch (OperationCanceledException cex)
                {
                    this.Logger.Error(cex, $"{appInitializerIdentifier} was canceled during initialization. The current operation will be interrupted.");
                    throw;
                }
                catch (Exception ex)
                {
                    var initializerKind = appInitializer.Item2.OptionalService ? "optional" : "required";
                    this.Logger.Error(ex, $"{appInitializerIdentifier} ({initializerKind}) failed to initialize. See the inner exception for more details.");
                    // interrupt the bootstrapping if a required initializer failed to start.
                    if (!appInitializer.Item2.OptionalService)
                    {
                        throw;
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}