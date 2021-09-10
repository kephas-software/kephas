// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchestrationAppSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Configuration
{
    using Kephas.Application;
    using Kephas.Application.Configuration;
    using Kephas.Collections;
    using Kephas.Configuration;
    using Kephas.Services;

    /// <summary>
    /// The orchestration implementation of the <see cref="IAppSettingsProvider"/>
    /// </summary>
    [OverridePriority(Priority.BelowNormal)]
    public class OrchestrationAppSettingsProvider : IAppSettingsProvider
    {
        private readonly IAppRuntime appRuntime;
        private readonly IConfiguration<OrchestrationSettings> systemConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestrationAppSettingsProvider"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="systemConfiguration">The system configuration.</param>
        public OrchestrationAppSettingsProvider(IAppRuntime appRuntime, IConfiguration<OrchestrationSettings> systemConfiguration)
        {
            this.appRuntime = appRuntime;
            this.systemConfiguration = systemConfiguration;
        }

        /// <summary>
        /// Gets the application settings for the executing instance.
        /// </summary>
        /// <returns>The application settings.</returns>
        public AppSettings? GetAppSettings()
            => this.GetAppSettings(this.appRuntime.GetAppId()!);

        /// <summary>
        /// Gets the application settings for the provided application.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <returns>The application settings.</returns>
        public AppSettings? GetAppSettings(string appId)
        {
            var systemSettings = this.systemConfiguration.GetSettings();
            return systemSettings.Instances?.TryGetValue(appId);
        }
    }
}