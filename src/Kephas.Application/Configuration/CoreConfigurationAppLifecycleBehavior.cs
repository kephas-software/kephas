// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoreConfigurationAppLifecycleBehavior.cs" company="Kephas Software SRL">
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
        private readonly IAppContext appContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreConfigurationAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="coreConfiguration">The core configuration.</param>
        /// <param name="appContext">The application context.</param>
        public CoreConfigurationAppLifecycleBehavior(
            IConfiguration<CoreSettings> coreConfiguration,
            IAppContext appContext)
        {
            this.coreConfiguration = coreConfiguration ?? throw new ArgumentNullException(nameof(coreConfiguration));
            this.appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task<IOperationResult> BeforeAppInitializeAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var settings = this.coreConfiguration.GetSettings(this.appContext);
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
    }
}
