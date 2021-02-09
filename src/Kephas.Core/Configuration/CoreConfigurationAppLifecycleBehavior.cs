// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the configuration application lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Core configuration application lifecycle behavior.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    public class CoreConfigurationAppLifecycleBehavior : Loggable, IAppLifecycleBehavior
    {
        private readonly IConfiguration<CoreSettings> coreConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreConfigurationAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="coreConfiguration">The core configuration.</param>
        public CoreConfigurationAppLifecycleBehavior(IConfiguration<CoreSettings> coreConfiguration)
        {
            this.coreConfiguration = coreConfiguration;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task<IOperationResult> BeforeAppInitializeAsync(
            IContext appContext,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var settings = this.coreConfiguration.GetSettings();
                if (settings == null)
                {
                    return Task.FromResult((IOperationResult)true.ToOperationResult());
                }

                if (settings.Task != null)
                {
                    TaskHelper.DefaultTimeout = settings.Task.DefaultTimeout ?? TaskHelper.DefaultTimeout;
                    TaskHelper.DefaultWaitMilliseconds = settings.Task.DefaultWaitMilliseconds ?? TaskHelper.DefaultWaitMilliseconds;
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while trying to set core default values.");
            }

            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task<IOperationResult> AfterAppInitializeAsync(
            IContext appContext,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task<IOperationResult> BeforeAppFinalizeAsync(
            IContext appContext,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task<IOperationResult> AfterAppFinalizeAsync(
            IContext appContext,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }
    }
}
