// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
                this.Log(LogLevel.Fatal, ex, Strings.App_BootstrapAsync_ErrorDuringConfiguration_Exception);
                throw;
            }

            try
            {
                this.Log(LogLevel.Info, null, Strings.App_BootstrapAsync_InitializingAppManager_Message);
                var appContext = await this.InitializeAppManagerAsync(appArgs, ambientServices, cancellationToken);

                this.Log(LogLevel.Info, null, Strings.App_BootstrapAsync_StartComplete_Message);

                return appContext;
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Fatal, ex, Strings.App_BootstrapAsync_ErrorDuringInitialization_Exception);

                try
                {
                    await this.ShutdownAsync(ambientServices, cancellationToken).PreserveThreadContext();
                }
                catch (Exception shutdownEx)
                {
                    this.Log(LogLevel.Fatal, shutdownEx, Strings.App_BootstrapAsync_ErrorDuringForcedShutdown_Exception);
                }

                throw;
            }
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
            try
            {
                this.Log(LogLevel.Info, null, Strings.App_ShutdownAsync_ShuttingDown_Message);

                ambientServices = ambientServices ?? AmbientServices.Instance;
                var appContext = await this.FinalizeAppManagerAsync(ambientServices, cancellationToken);

                this.Log(LogLevel.Info, null, Strings.App_ShutdownAsync_Complete_Message);

                return appContext;
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Fatal, ex, Strings.App_ShutdownAsync_ErrorDuringFinalization_Exception);
                throw;
            }
        }

        /// <summary>
        /// Configures the ambient services asynchronously.
        /// </summary>
        /// <remarks>
        /// This method should be overwritten to provide a meaningful content.
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
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the <see cref="IAppContext"/>.
        /// </returns>
        protected virtual async Task<IAppContext> InitializeAppManagerAsync(string[] appArgs, IAmbientServices ambientServices, CancellationToken cancellationToken)
        {
            var container = ambientServices.CompositionContainer;
            var appContext = this.CreateAppContext(appArgs, ambientServices);
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
                signalShutdown: c => this.ShutdownAsync(ambientServices));
            return appContext;
        }

        /// <summary>
        /// Logs the provided information.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        protected virtual void Log(LogLevel level, Exception exception, string messageFormat, params object[] args)
        {
            this.Logger?.Log(level, exception, messageFormat, args);
        }
    }
}