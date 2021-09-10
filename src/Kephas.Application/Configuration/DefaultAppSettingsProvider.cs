﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kephas.Application.Configuration
{
    using Kephas.Configuration;
    using Kephas.Services;

    /// <summary>
    /// The default implementation of the <see cref="IAppSettingsProvider"/>
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultAppSettingsProvider : IAppSettingsProvider
    {
        private readonly IAppRuntime appRuntime;
        private readonly IConfiguration<AppSettings> appConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAppSettingsProvider"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="appConfiguration">The system configuration.</param>
        public DefaultAppSettingsProvider(IAppRuntime appRuntime, IConfiguration<AppSettings> appConfiguration)
        {
            this.appRuntime = appRuntime;
            this.appConfiguration = appConfiguration;
        }

        /// <summary>
        /// Gets the application settings for the executing instance.
        /// </summary>
        /// <returns>The application settings.</returns>
        public AppSettings? GetAppSettings()
            => this.appConfiguration.GetSettings();

        /// <summary>
        /// Gets the application settings for the provided application.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <returns>The application settings.</returns>
        public AppSettings? GetAppSettings(string appId)
        {
            var thisAppId = this.appRuntime.GetAppId();
            if (!string.Equals(thisAppId, appId, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ApplicationException($"{nameof(AppSettings)} for '{appId}' was requested. However, the current application is '{thisAppId}'. Possible cause: missing Kephas.Orchestration assembly.");
            }

            var appSettings = this.appConfiguration.GetSettings();
            return appSettings;
        }
    }
}