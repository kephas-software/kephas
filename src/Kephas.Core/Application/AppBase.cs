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
        /// Starts the application asynchronously.
        /// </summary>
        /// <param name="appArgs">The application arguments (optional).</param>
        /// <param name="ambientServices">The ambient services (optional). If not provided then <see cref="AmbientServices.Instance"/> is considered.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result that yields the provided or the created <see cref="IAmbientServices"/>.
        /// </returns>
        public virtual async Task<IAmbientServices> StartApplicationAsync(
            string[] appArgs = null,
            IAmbientServices ambientServices = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                this.Log(LogLevel.Info, null, Strings.App_StartApplication_Starting_Message);

                ambientServices = ambientServices ?? AmbientServices.Instance;

                this.Log(LogLevel.Info, null, Strings.App_StartApplication_ConfiguringAmbientServices_Message);
                var ambientServicesBuilder = new AmbientServicesBuilder(ambientServices);
                await this.ConfigureAmbientServicesAsync(appArgs, ambientServicesBuilder, cancellationToken).PreserveThreadContext();

                this.Logger = this.Logger ?? ambientServices.GetLogger(this.GetType());

                this.Log(LogLevel.Info, null, Strings.App_StartApplication_InitializingAppManager_Message);
                await this.InitializeAppManagerAsync(appArgs, ambientServices, cancellationToken);

                this.Log(LogLevel.Info, null, Strings.App_StartApplication_StartComplete_Message);
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Fatal, ex, Strings.App_StartApplication_ErrorDuringInitialization_Exception);
                throw;
            }

            try
            {
                await this.RunAsync(appArgs, ambientServices, cancellationToken).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Error, ex, Strings.App_RunApplication_Exception);
            }
            finally
            {
                await this.StopApplicationAsync(ambientServices, cancellationToken).PreserveThreadContext();
            }

            return ambientServices;
        }

        /// <summary>
        /// Stops the application asynchronously.
        /// </summary>
        /// <param name="ambientServices">The ambient services (optional). If not provided then <see cref="AmbientServices.Instance"/> is considered.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public virtual async Task StopApplicationAsync(
            IAmbientServices ambientServices = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                this.Log(LogLevel.Info, null, Strings.App_StopApplication_Stopping_Message);

                ambientServices = ambientServices ?? AmbientServices.Instance;

                var appContext = ambientServices.CompositionContainer.GetExport<IAppContext>();
                var appManager = ambientServices.CompositionContainer.GetExport<IAppManager>();

                await appManager.FinalizeAppAsync(appContext, cancellationToken).PreserveThreadContext();
                this.Log(LogLevel.Info, null, Strings.App_StopApplication_StopComplete_Message);
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Fatal, ex, Strings.App_StopApplication_ErrorDuringFinalization_Exception);
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
        /// Executes the application main functionality asynchronously.
        /// </summary>
        /// <remarks>
        /// This method should be overwritten to provide a meaningful content.
        /// </remarks>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServices">The configured ambient services.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        protected virtual Task RunAsync(
            string[] appArgs,
            IAmbientServices ambientServices,
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
        /// The asynchronous result.
        /// </returns>
        protected virtual async Task InitializeAppManagerAsync(string[] appArgs, IAmbientServices ambientServices, CancellationToken cancellationToken)
        {
            var container = ambientServices.CompositionContainer;
            var appContext = this.CreateAppContext(appArgs, ambientServices);
            var appManager = container.GetExport<IAppManager>();
            await appManager.InitializeAppAsync(appContext, cancellationToken).PreserveThreadContext();
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
            var appManifest = ambientServices.CompositionContainer.GetExport<IAppManifest>();
            var appContext = new AppContext(ambientServices, appManifest, appArgs);
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