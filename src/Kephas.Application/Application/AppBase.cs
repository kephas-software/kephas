// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Resources;
    using Kephas.Services.Builder;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for the application's root.
    /// </summary>
    public abstract class AppBase : IApp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppBase"/> class.
        /// </summary>
        /// <param name="appArgs">Optional. The application arguments.</param>
        /// <param name="appLifetimeTokenSource">Optional. The cancellation token source used to stop the application.</param>
        protected AppBase(
            IAppArgs? appArgs = null,
            CancellationTokenSource? appLifetimeTokenSource = null)
        {
            this.AppArgs = appArgs ?? new AppArgs();
            this.ServicesBuilder = new AppServiceCollectionBuilder()
                .AddAppArgs(this.AppArgs);
            this.AppLifetimeTokenSource = appLifetimeTokenSource;
            AppDomain.CurrentDomain.UnhandledException += this.OnCurrentDomainUnhandledException;
        }

        /// <summary>
        /// Gets the application services builder.
        /// </summary>
        /// <value>
        /// The application services builder.
        /// </value>
        public IAppServiceCollectionBuilder ServicesBuilder { get; }

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/>.
        /// </summary>
        public IServiceProvider? ServiceProvider { get; private set; }

        /// <summary>
        /// Gets a context for the application.
        /// </summary>
        /// <value>
        /// The application context.
        /// </value>
        public virtual IAppContext? AppContext { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the application is running.
        /// </summary>
        public virtual bool IsRunning { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the application is shutting down.
        /// </summary>
        public virtual bool IsShuttingDown { get; protected set; }

        /// <summary>
        /// Gets the application arguments.
        /// </summary>
        protected IAppArgs AppArgs { get; }

        /// <summary>
        /// Gets a value indicating whether the application is configured.
        /// </summary>
        protected bool IsConfigured { get; private set; }

        /// <summary>
        /// Gets or sets the cancellation token source used to stop the application.
        /// </summary>
        protected CancellationTokenSource? AppLifetimeTokenSource { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected ILogger? Logger { get; set; }

        /// <summary>
        /// Gets or sets the callback for the services configuration.
        /// </summary>
        protected Action<IAppServiceCollectionBuilder>? ServicesConfiguration { get; set; }

        /// <summary>
        /// Runs the application asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields the <see cref="IAppContext"/>.
        /// </returns>
        public virtual async Task<AppRunResult> RunAsync(CancellationToken cancellationToken = default)
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException("The application is already running.");
            }

            this.Log(LogLevel.Info, null, Strings.App_RunAsync_Bootstrapping_Message);

            this.IsRunning = true;

            await Task.Yield();

            this.ConfigureServices();

            this.ServiceProvider = this.BuildServiceProvider(this.ServicesBuilder);

            this.Logger ??= this.ServiceProvider.GetRequiredService<ILogManager>().GetLogger(this.GetType());

            await this.InitializeAppManagerAsync(this.AppContext!, cancellationToken).PreserveThreadContext();

            this.AppLifetimeTokenSource ??= new CancellationTokenSource();
            var instruction = await this.RunMainLoop(this.AppLifetimeTokenSource.Token).PreserveThreadContext();

            if (instruction != AppShutdownInstruction.Shutdown)
            {
                return new AppRunResult(this.AppContext, instruction);
            }

            try
            {
                await this.ShutdownAsync(cancellationToken).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(ex, "Abnormal application termination.");
                this.AppContext!.Exception = ex;
                return new AppRunResult(null, instruction);
            }
            finally
            {
                this.IsRunning = false;
            }

            return new AppRunResult(this.AppContext, instruction);
        }

        /// <summary>
        /// Shuts down the application asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public virtual async Task ShutdownAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                this.Log(LogLevel.Info, null, "Entering the shutdown procedure...");

                if (this.IsShuttingDown)
                {
                    throw new InvalidOperationException("The application is shutting down.");
                }

                this.IsShuttingDown = true;

                var appContext = await this.FinalizeAppManagerAsync(cancellationToken).PreserveThreadContext();
                appContext.Dispose();

                this.Log(LogLevel.Info, null, "Completed the shutdown procedure.");
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Fatal, ex, "Errors occurred during shutdown procedure, gracefully terminating the application.");
            }
            finally
            {
                this.IsShuttingDown = false;
                this.IsRunning = false;
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.</summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (this.IsRunning is false)
            {
                return;
            }

            await this.ShutdownAsync();
        }

        /// <summary>
        /// The <see cref="ConfigureServices"/> is called before the application manager is initialized.
        /// Initializes the application prerequisites: the ambient services, the application context
        /// registration, its own logger, and other. In the end, the <see cref="BuildServiceProvider"/> method is called
        /// to complete the service registration and build the injector.
        /// </summary>
        /// <returns>
        /// True if the initialization was performed, false if it was ignored because of subsequent calls.
        /// </returns>
        protected virtual bool ConfigureServices()
        {
            if (this.IsConfigured)
            {
                this.Log(LogLevel.Info, null, "Already configured, skipping configuration.");
                return false;
            }

            try
            {
                this.Log(LogLevel.Info, null, Strings.App_RunAsync_ConfiguringAmbientServices_Message);

                this.ServicesConfiguration?.Invoke(this.ServicesBuilder);

                var ambientServices = this.ServicesBuilder.AmbientServices;
                this.Logger ??= this.ServicesBuilder.Logger
                                ?? ambientServices.TryGetServiceInstance<ILogManager>()?.GetLogger(this.GetType());

                // it is important to create the app context before initializing the application manager
                // and after configuring the ambient services and the logger, as it may
                // use registered services.
                this.AppContext = this.CreateAppContext();

                // require the AppContext to be computed each time, so that if it is called
                // too early, to be able to still get it at a later time.
                // registers the application context as a global service, so that other services can benefit from it.
                ambientServices.Add(this.AppContext);

                this.Log(LogLevel.Info, null, Resources.Strings.AppBase_ConfigureSuccessful);
            }
            catch (Exception ex)
            {
                var bootstrapException = new BootstrapException(
                    Strings.App_RunAsync_ErrorDuringConfiguration_Exception,
                    this.ServicesBuilder,
                    ex)
                {
                    AppContext = this.AppContext,
                };
                this.Log((LogLevel)bootstrapException.Severity, bootstrapException);
                throw;
            }

            return this.IsConfigured = true;
        }

        /// <summary>
        /// Executes the application's main loop asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the shutdown instruction.
        /// </returns>
        protected virtual async Task<AppShutdownInstruction> RunMainLoop(CancellationToken cancellationToken)
        {
            try
            {
                var (result, instruction) = await this.Main(cancellationToken);

                this.AppContext!.AppResult = result;

                return instruction;
            }
            catch (OperationCanceledException cex)
                when (cex.CancellationToken == cancellationToken)
            {
                this.Logger.Info("Shutdown triggered by cancelling the application lifetime token.");
                return AppShutdownInstruction.Shutdown;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error during waiting for shutdown signal.");
                this.AppContext!.Exception = ex;

                return AppShutdownInstruction.Shutdown;
            }
        }

        /// <summary>
        /// The main loop.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task yielding the <see cref="MainLoopResult"/>.</returns>
        protected virtual async Task<MainLoopResult> Main(CancellationToken cancellationToken)
        {
            var mainLoop = this.ServiceProvider!.TryResolve<IAppMainLoop>();
            if (mainLoop != null)
            {
                return await mainLoop.Main(cancellationToken).PreserveThreadContext();
            }

            return new MainLoopResult(0.ToOperationResult(), AppShutdownInstruction.Shutdown);
        }

        /// <summary>
        /// Handles the unhandled exception event.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event information to send to registered event handlers.</param>
        protected virtual void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var messageFormat = e.IsTerminating
                                    ? Strings.UnhandledException_Terminating_Message
                                    : Strings.UnhandledException_InProcess_Message;
            this.Log(
                LogLevel.Fatal,
                e.ExceptionObject as Exception,
                messageFormat);
        }

        /// <summary>
        /// This is the last step in the app's configuration, when all the services are set up
        /// and the container is built. For inheritors, this is the last place where services can
        /// be added before calling. By default, it only builds the Lite container, but any other container adapter
        /// can be used, like Autofac or System.Composition.
        /// </summary>
        /// <remarks>
        /// Override this method to initialize the startup services, like log manager and configuration manager.
        /// </remarks>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <returns>The service provider.</returns>
        protected abstract IServiceProvider BuildServiceProvider(IAppServiceCollectionBuilder servicesBuilder);

        /// <summary>
        /// Initializes the application manager asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the <see cref="IAppContext"/>.
        /// </returns>
        protected virtual async Task<IAppContext> InitializeAppManagerAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            try
            {
                this.Log(LogLevel.Info, null, Strings.App_RunAsync_InitializingAppManager_Message);

                var container = appContext.ServiceProvider;
                var appManager = container.Resolve<IAppManager>();

                await appManager.InitializeAsync(appContext, cancellationToken).PreserveThreadContext();

                this.Log(LogLevel.Info, null, Strings.App_RunAsync_StartComplete_Message);

                return appContext;
            }
            catch (Exception ex)
            {
                var bootstrapException = new BootstrapException(
                    Strings.App_RunAsync_ErrorDuringConfiguration_Exception,
                    this.ServicesBuilder,
                    ex)
                {
                    AppContext = appContext,
                };

                appContext.Exception = bootstrapException;

                this.Log(LogLevel.Fatal, bootstrapException);

                try
                {
                    await this.ShutdownAsync(cancellationToken).PreserveThreadContext();
                }
                catch (Exception shutdownEx)
                {
                    this.Log(LogLevel.Fatal, shutdownEx, Strings.App_RunAsync_ErrorDuringForcedShutdown_Exception);
                }

                throw bootstrapException;
            }
        }

        /// <summary>
        /// Finalizes the application manager asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the <see cref="IAppContext"/>.
        /// </returns>
        protected virtual async Task<IAppContext> FinalizeAppManagerAsync(CancellationToken cancellationToken)
        {
            IAppContext? appContext = null;

            try
            {
                this.Log(LogLevel.Info, null, Strings.App_ShutdownAsync_ShuttingDown_Message);

                appContext = this.ServiceProvider!.Resolve<IAppContext>();
                var appManager = this.ServiceProvider!.Resolve<IAppManager>();

                await appManager.FinalizeAsync(appContext, cancellationToken).PreserveThreadContext();

                this.Log(LogLevel.Info, null, Strings.App_ShutdownAsync_Complete_Message);

                return appContext;
            }
            catch (Exception ex)
            {
                var shutdownException = new ShutdownException(Strings.App_ShutdownAsync_ErrorDuringFinalization_Exception, ex)
                {
                    AmbientServices = this.ServicesBuilder.AmbientServices,
                    AppContext = appContext ?? this.AppContext,
                };
                this.Log(LogLevel.Fatal, shutdownException);
                throw shutdownException;
            }
        }

        /// <summary>
        /// Creates the application context.
        /// </summary>
        /// <returns>
        /// The new application context.
        /// </returns>
        protected virtual IAppContext CreateAppContext()
        {
            var appContext = new AppContext(this.ServicesBuilder, this.AppArgs)
            {
                Logger = this.Logger,
            };
            return appContext;
        }

        /// <summary>
        /// Logs the provided information.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// True if the log operation succeeded, false if it failed.
        /// </returns>
        protected virtual bool Log(LogLevel level, Exception? exception, string? messageFormat = null, params object[] args)
        {
            return this.Logger?.Log(level, exception, messageFormat ?? exception?.Message, args) ?? false;
        }
    }
}