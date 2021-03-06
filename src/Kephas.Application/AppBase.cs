﻿// --------------------------------------------------------------------------------------------------------------------
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
    using Kephas.Resources;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for the application's root.
    /// </summary>
    /// <remarks>
    /// You should inherit this class and override at least the <see cref="BuildServicesContainer"/> method.
    /// </remarks>
    public abstract class AppBase
    {
        private readonly Action<IAmbientServices>? containerBuilder;
        private bool isConfigured;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppBase"/> class.
        /// </summary>
        /// <param name="ambientServices">Optional. The ambient services. If not provided then
        ///                               a new instance of <see cref="Kephas.AmbientServices"/> will be created and used.</param>
        /// <param name="appLifetimeTokenSource">Optional. The cancellation token source used to stop the application.</param>
        /// <param name="containerBuilder">Optional. The container builder.</param>
        protected AppBase(IAmbientServices? ambientServices = null, CancellationTokenSource? appLifetimeTokenSource = null, Action<IAmbientServices>? containerBuilder = null)
        {
            this.AmbientServices = ambientServices ?? new AmbientServices();
            this.AppLifetimeTokenSource = appLifetimeTokenSource;
            this.containerBuilder = containerBuilder;
            AppDomain.CurrentDomain.UnhandledException += this.OnCurrentDomainUnhandledException;
        }

        /// <summary>
        /// Gets or sets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public IAmbientServices AmbientServices { get; protected set; }

        /// <summary>
        /// Gets a context for the application.
        /// </summary>
        /// <value>
        /// The application context.
        /// </value>
        public IAppContext? AppContext { get; private set; }

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
        /// Bootstraps the application asynchronously.
        /// </summary>
        /// <param name="appArgs">Optional. The application arguments.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields the <see cref="IAppContext"/>.
        /// </returns>
        public virtual async Task<(IAppContext? appContext, AppShutdownInstruction instruction)> BootstrapAsync(
            IAppArgs? appArgs = null,
            CancellationToken cancellationToken = default)
        {
            this.Log(LogLevel.Info, null, Strings.App_BootstrapAsync_Bootstrapping_Message);

            await Task.Yield();

            this.BeforeAppManagerInitialize(appArgs);

            await this.InitializeAppManagerAsync(this.AppContext, cancellationToken).PreserveThreadContext();

            this.AfterAppManagerInitialize();

            this.AppLifetimeTokenSource ??= new CancellationTokenSource();
            var instruction = await this.WaitForShutdownSignalAsync(this.AppLifetimeTokenSource.Token).PreserveThreadContext();

            if (instruction != AppShutdownInstruction.Shutdown)
            {
                return (this.AppContext, instruction);
            }

            try
            {
                await this.ShutdownAsync(cancellationToken).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(ex, "Abnormal application termination.");
                this.AppContext.Exception = ex;
                return (null, instruction);
            }

            return (this.AppContext, instruction);
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

                this.BeforeAppManagerFinalize();

                var appContext = await this.FinalizeAppManagerAsync(cancellationToken).PreserveThreadContext();
                appContext?.Dispose();

                this.Log(LogLevel.Info, null, "Completed the shutdown procedure.");
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Fatal, ex, "Errors occurred during shutdown procedure, gracefully terminating the application.");
            }
            finally
            {
                try
                {
                    this.AfterAppManagerFinalize();
                }
                catch
                {
                    // At this moment the loggers are disposed, do nothing
#if DEBUG
                    Debug.Assert(false, "Should not fail in finalizing prerequisites.");
#endif
                }
            }
        }

        /// <summary>
        /// The <see cref="BeforeAppManagerFinalize"/> is called before the application manager starts finalization.
        /// </summary>
        protected virtual void BeforeAppManagerFinalize()
        {
        }

        /// <summary>
        /// The <see cref="AfterAppManagerFinalize"/> is called after the application manager completed finalization.
        /// It disposes the composition container and the ambient services.
        /// </summary>
        protected virtual void AfterAppManagerFinalize()
        {
            this.AmbientServices?.CompositionContainer.Dispose();
            this.AmbientServices?.Dispose();
        }

        /// <summary>
        /// The <see cref="BeforeAppManagerInitialize"/> is called before the application manager is initialized.
        /// Initializes the application prerequisites: the ambient services, the application context
        /// registration, its own logger, and other. In the end, the <see cref="BuildServicesContainer"/> method is called
        /// to complete the service registration and build the composition container.
        /// </summary>
        /// <param name="appArgs">The application arguments.</param>
        /// <returns>
        /// True if the initialization was performed, false if it was ignored because of subsequent calls.
        /// </returns>
        protected virtual bool BeforeAppManagerInitialize(IAppArgs? appArgs)
        {
            if (this.isConfigured)
            {
                return false;
            }

            try
            {
                this.Log(LogLevel.Info, null, Strings.App_BootstrapAsync_ConfiguringAmbientServices_Message);

                // require the AppContext to be computed each time, so that if it is called
                // to early, to be able to still get it at a later time.
                // registers the application context as a global service, so that other services can benefit from it.
                this.AmbientServices.Register<IAppContext>(b => b.WithFactory(ctx => this.AppContext).AsTransient());

                this.AmbientServices.RegisterAppArgs(appArgs);

                this.BuildServicesContainer(this.AmbientServices);

                this.Logger ??= this.AmbientServices.GetLogger(this.GetType());

                // it is important to create the app context before initializing the application manager
                // and after configuring the ambient services and the logger, as it may
                // use registered services.
                this.AppContext = this.CreateAppContext(this.AmbientServices);
            }
            catch (Exception ex)
            {
                var bootstrapException = new BootstrapException(Strings.App_BootstrapAsync_ErrorDuringConfiguration_Exception, ex)
                {
                    AmbientServices = this.AmbientServices,
                };
                this.Log((LogLevel)bootstrapException.Severity, bootstrapException);
                throw;
            }

            return this.isConfigured = true;
        }

        /// <summary>
        /// The <see cref="AfterAppManagerInitialize"/> is called after the application manager completed initialization.
        /// </summary>
        protected virtual void AfterAppManagerInitialize()
        {
        }

        /// <summary>
        /// Waits for the shutdown signal asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the shutdown instruction.
        /// </returns>
        protected virtual async Task<AppShutdownInstruction> WaitForShutdownSignalAsync(CancellationToken cancellationToken)
        {
            try
            {
                var container = this.AmbientServices.CompositionContainer;
                var terminationAwaiter = container.GetExport<IAppShutdownAwaiter>();
                var (result, instruction) = await terminationAwaiter.WaitForShutdownSignalAsync(cancellationToken)
                    .PreserveThreadContext();
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
                this.AppContext.Exception = ex;

                return AppShutdownInstruction.Shutdown;
            }
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
        /// can be used, like Autofac or System.Composition (MEF 2).
        /// </summary>
        /// <remarks>
        /// Override this method to initialize the startup services, like log manager and configuration manager.
        /// </remarks>
        /// <param name="ambientServices">The ambient services.</param>
        protected virtual void BuildServicesContainer(IAmbientServices ambientServices)
        {
            if (this.containerBuilder != null)
            {
                this.containerBuilder(ambientServices);
            }
            else
            {
                ambientServices.BuildWithLite();
            }
        }

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
                this.Log(LogLevel.Info, null, Strings.App_BootstrapAsync_InitializingAppManager_Message);

                var container = appContext.CompositionContext;
                var appManager = container.GetExport<IAppManager>();

                await appManager.InitializeAppAsync(appContext, cancellationToken).PreserveThreadContext();

                this.Log(LogLevel.Info, null, Strings.App_BootstrapAsync_StartComplete_Message);

                return appContext;
            }
            catch (Exception ex)
            {
                var bootstrapException = new BootstrapException(Strings.App_BootstrapAsync_ErrorDuringConfiguration_Exception, ex)
                {
                    AppContext = appContext,
                    AmbientServices = this.AmbientServices,
                };
                if (appContext != null)
                {
                    appContext.Exception = bootstrapException;
                }

                this.Log(LogLevel.Fatal, bootstrapException);

                try
                {
                    await this.ShutdownAsync(cancellationToken).PreserveThreadContext();
                }
                catch (Exception shutdownEx)
                {
                    this.Log(LogLevel.Fatal, shutdownEx, Strings.App_BootstrapAsync_ErrorDuringForcedShutdown_Exception);
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

                var container = this.AmbientServices.CompositionContainer;
                appContext = container.GetExport<IAppContext>();
                var appManager = container.GetExport<IAppManager>();

                await appManager.FinalizeAppAsync(appContext, cancellationToken).PreserveThreadContext();

                this.Log(LogLevel.Info, null, Strings.App_ShutdownAsync_Complete_Message);

                return appContext;
            }
            catch (Exception ex)
            {
                var shutdownException = new ShutdownException(Strings.App_ShutdownAsync_ErrorDuringFinalization_Exception, ex)
                {
                    AmbientServices = this.AmbientServices,
                    AppContext = appContext ?? this.AppContext,
                };
                this.Log(LogLevel.Fatal, shutdownException);
                throw shutdownException;
            }
        }

        /// <summary>
        /// Creates the application context.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// The new application context.
        /// </returns>
        protected virtual IAppContext CreateAppContext(IAmbientServices ambientServices)
        {
            var appContext = new AppContext(
                                     ambientServices,
                                     appArgs: ambientServices.GetService<IAppArgs>())
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