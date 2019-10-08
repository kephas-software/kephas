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
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Resources;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for the application's root.
    /// </summary>
    /// <remarks>
    /// You should inherit this class and override at least the <see cref="ConfigureAmbientServices"/> method.
    /// </remarks>
    public abstract class AppBase
    {
        private bool prerequisitesInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppBase"/> class.
        /// </summary>
        /// <param name="ambientServices">Optional. The ambient services. If not provided then
        ///                               <see cref="AmbientServices.Instance"/> is considered.</param>
        protected AppBase(IAmbientServices ambientServices = null)
        {
            this.AmbientServices = ambientServices ?? Kephas.AmbientServices.Instance;
            AppDomain.CurrentDomain.UnhandledException += this.OnCurrentDomainUnhandledException;
        }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public IAmbientServices AmbientServices { get; }

        /// <summary>
        /// Gets a context for the application.
        /// </summary>
        /// <value>
        /// The application context.
        /// </value>
        public IAppContext AppContext { get; private set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected ILogger Logger { get; set; }

        /// <summary>
        /// Bootstraps the application asynchronously.
        /// </summary>
        /// <param name="rawAppArgs">Optional. The application arguments.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields the <see cref="IAppContext"/>.
        /// </returns>
        public virtual async Task<IAppContext> BootstrapAsync(
            string[] rawAppArgs = null,
            CancellationToken cancellationToken = default)
        {
            this.Log(LogLevel.Info, null, Strings.App_BootstrapAsync_Bootstrapping_Message);

            this.InitializePrerequisites(rawAppArgs);

            await this.InitializeAppManagerAsync(this.AppContext, cancellationToken).PreserveThreadContext();

            try
            {
                var container = this.AmbientServices.CompositionContainer;
                var terminationAwaiter = container.GetExport<IAppShutdownAwaiter>();
                var (result, instruction) = await terminationAwaiter.WaitForShutdownSignalAsync(cancellationToken).PreserveThreadContext();
                this.AppContext.AppResult = result;

                if (instruction == AppShutdownInstruction.Shutdown)
                {
                    await this.ShutdownAsync(cancellationToken).PreserveThreadContext();
                }
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(ex, "Abnormal application termination.");
                this.AppContext.Exception = ex;
                return null;
            }

            return this.AppContext;
        }

        /// <summary>
        /// Shuts down the application asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of the <see cref="IAppContext"/>.
        /// </returns>
        public virtual async Task<IAppContext> ShutdownAsync(CancellationToken cancellationToken = default)
        {
            var appContext = await this.FinalizeAppManagerAsync(cancellationToken).PreserveThreadContext();
            appContext?.Dispose();
            return appContext;
        }

        /// <summary>
        /// Initializes the application prerequisites: the ambient services, the application context
        /// registration, the logger, other.
        /// </summary>
        /// <param name="rawAppArgs">The application arguments.</param>
        /// <returns>
        /// True if the initialization was performed, false if it was ignored because of subsequent calls.
        /// </returns>
        protected virtual bool InitializePrerequisites(string[] rawAppArgs)
        {
            if (this.prerequisitesInitialized)
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

                var appArgs = rawAppArgs == null ? new AppArgs() : new AppArgs(rawAppArgs);
                this.AmbientServices.Register<IAppArgs>(b => b.WithInstance(appArgs));
                this.ConfigureAmbientServices(this.AmbientServices);

                this.Logger = this.Logger ?? this.AmbientServices.GetLogger(this.GetType());

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

            return this.prerequisitesInitialized = true;
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
        /// Configures the ambient services asynchronously.
        /// </summary>
        /// <remarks>
        /// Override this method to initialize the startup services, like log manager and configuration manager.
        /// </remarks>
        /// <param name="ambientServices">The ambient services.</param>
        protected abstract void ConfigureAmbientServices(IAmbientServices ambientServices);

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
            IAppContext appContext = null;

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
        protected virtual void Log(LogLevel level, Exception exception, string messageFormat = null, params object[] args)
        {
            this.Logger?.Log(level, exception, messageFormat ?? exception.Message, args);
        }
    }
}