namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
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
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            this.Logger.Info($"The bootstrapper started at {DateTimeOffset.Now:s}.");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await this.BeforeStartAsync(appContext, cancellationToken).WithServerContext();
                cancellationToken.ThrowIfCancellationRequested();

                await this.RunInitializersAsync(appContext, cancellationToken).WithServerContext();
                cancellationToken.ThrowIfCancellationRequested();

                await this.AfterStartAsync(appContext, cancellationToken).WithServerContext();
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                stopwatch.Stop();
                this.Logger.Error($"The boostrapper start procedure was canceled at {DateTimeOffset.Now:s}. Elapsed: {stopwatch.Elapsed:c}.");
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                this.Logger.Error($"The boostrapper encountered an exception while starting at {DateTimeOffset.Now:s}. Elapsed: {stopwatch.Elapsed:c}.", ex);
                throw;
            }

            stopwatch.Stop();
            this.Logger.Info($"Boostrapper ended at {DateTimeOffset.Now:s}. Elapsed: {stopwatch.Elapsed:c}.");
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
            return Task.FromResult(0);
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
            return Task.FromResult(0);
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
                var itemStopwatch = new Stopwatch();
                itemStopwatch.Start();
                try
                {
                    this.Logger.Info($"AppInitializer '{initializerType}' started.");
                    await appInitializer.Item1.InitializeAsync(appContext, cancellationToken).WithServerContext();
                    itemStopwatch.Stop();
                    this.Logger.Info($"AppInitializer '{initializerType}' completed initialization. Elapsed {itemStopwatch.Elapsed:c}");
                }
                catch (OperationCanceledException cex)
                {
                    this.Logger.Error($"AppInitializer '{initializerType}' was canceled during initialization. The current operation will be interrupted.", cex);
                    throw;
                }
                catch (Exception ex)
                {
                    var initializerKind = appInitializer.Item2.OptionalService ? "optional" : "required";
                    this.Logger.Error($"AppInitializer '{initializerType}' ({initializerKind}) failed to initialize. See the inner exception for more details.", ex);
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