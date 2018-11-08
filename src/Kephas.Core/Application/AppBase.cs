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
    /// You should inherit this class and override at least the <see cref="ConfigureAmbientServicesAsync"/> method.
    /// </remarks>
    public abstract class AppBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppBase"/> class.
        /// </summary>
        protected AppBase()
        {
            AppDomain.CurrentDomain.UnhandledException += this.OnCurrentDomainUnhandledException;
        }

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
        /// <param name="appArgs">The application arguments (optional).</param>
        /// <param name="ambientServices">The ambient services (optional). If not provided then <see cref="AmbientServices.Instance"/> is considered.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result that yields the <see cref="IAppContext"/>.
        /// </returns>
        public virtual async Task<IAppContext> BootstrapAsync(
            string[] appArgs = null,
            IAmbientServices ambientServices = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                this.Log(LogLevel.Info, null, Strings.App_BootstrapAsync_Bootstrapping_Message);

                ambientServices = ambientServices ?? AmbientServices.Instance;

                this.Log(LogLevel.Info, null, Strings.App_BootstrapAsync_ConfiguringAmbientServices_Message);
                var ambientServicesBuilder = new AmbientServicesBuilder(ambientServices);
                await this.ConfigureAmbientServicesAsync(appArgs, ambientServicesBuilder, cancellationToken).PreserveThreadContext();

                this.Logger = this.Logger ?? ambientServices.GetLogger(this.GetType());
            }
            catch (Exception ex)
            {
                var bootstrapException = new BootstrapException(Strings.App_BootstrapAsync_ErrorDuringConfiguration_Exception, ex)
                {
                    AmbientServices = ambientServices,
                };
                this.Log((LogLevel)bootstrapException.Severity, bootstrapException);
                throw;
            }

            IAppContext appContext = null;
            try
            {
                this.Log(LogLevel.Info, null, Strings.App_BootstrapAsync_InitializingAppManager_Message);
                appContext = this.CreateAppContext(appArgs, ambientServices);

                // registers the application context as a global service, so that other services can benefit from it.
                // it is important to do it before initializing the application manager.
                ambientServices.RegisterService(appContext);
                await this.InitializeAppManagerAsync(appContext, cancellationToken);

                this.Log(LogLevel.Info, null, Strings.App_BootstrapAsync_StartComplete_Message);

                return appContext;
            }
            catch (Exception ex)
            {
                var bootstrapException = new BootstrapException(Strings.App_BootstrapAsync_ErrorDuringConfiguration_Exception, ex)
                {
                    AppContext = appContext,
                    AmbientServices = ambientServices,
                };
                if (appContext != null)
                {
                    appContext.Exception = bootstrapException;
                }

                this.Log(LogLevel.Fatal, bootstrapException);

                try
                {
                    await this.ShutdownAsync(ambientServices, cancellationToken).PreserveThreadContext();
                }
                catch (Exception shutdownEx)
                {
                    this.Log(LogLevel.Fatal, shutdownEx, Strings.App_BootstrapAsync_ErrorDuringForcedShutdown_Exception);
                }

                throw bootstrapException;
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
        /// Shuts down the application asynchronously.
        /// </summary>
        /// <param name="ambientServices">The ambient services (optional). If not provided then <see cref="AmbientServices.Instance"/> is considered.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the <see cref="IAppContext"/>.
        /// </returns>
        public virtual async Task<IAppContext> ShutdownAsync(
            IAmbientServices ambientServices = null,
            CancellationToken cancellationToken = default)
        {
            IAppContext appContext = null;
            ambientServices = ambientServices ?? AmbientServices.Instance;

            try
            {
                this.Log(LogLevel.Info, null, Strings.App_ShutdownAsync_ShuttingDown_Message);

                appContext = ambientServices.GetService<IAppContext>();
                appContext = await this.FinalizeAppManagerAsync(ambientServices, cancellationToken);

                this.Log(LogLevel.Info, null, Strings.App_ShutdownAsync_Complete_Message);

                return appContext;
            }
            catch (Exception ex)
            {
                var shutdownException = new ShutdownException(Strings.App_ShutdownAsync_ErrorDuringFinalization_Exception, ex)
                {
                    AmbientServices = ambientServices,
                    AppContext = appContext
                };
                this.Log(LogLevel.Fatal, shutdownException);
                throw shutdownException;
            }
        }

        /// <summary>
        /// Configures the ambient services asynchronously.
        /// </summary>
        /// <remarks>
        /// Override this method to initialize the startup services, like log manager and configuration manager.
        /// </remarks>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        protected virtual Task ConfigureAmbientServicesAsync(
            string[] appArgs,
            AmbientServicesBuilder ambientServicesBuilder,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
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
            var container = appContext.CompositionContext;
            var appManager = container.GetExport<IAppManager>();

            await appManager.InitializeAppAsync(appContext, cancellationToken).PreserveThreadContext();
            return appContext;
        }

        /// <summary>
        /// Finalizes the application manager asynchronously.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the <see cref="IAppContext"/>.
        /// </returns>
        protected virtual async Task<IAppContext> FinalizeAppManagerAsync(IAmbientServices ambientServices, CancellationToken cancellationToken)
        {
            var appContext = ambientServices.CompositionContainer.GetExport<IAppContext>();
            var appManager = ambientServices.CompositionContainer.GetExport<IAppManager>();

            await appManager.FinalizeAppAsync(appContext, cancellationToken).PreserveThreadContext();
            return appContext;
        }

        /// <summary>
        /// Creates the application context.
        /// </summary>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// The new application context.
        /// </returns>
        protected virtual IAppContext CreateAppContext(string[] appArgs, IAmbientServices ambientServices)
        {
            var appContext = new AppContext(
                                     ambientServices,
                                     appArgs: appArgs,
                                     signalShutdown: c => this.ShutdownAsync(ambientServices))
            {
                ContextLogger = this.Logger
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