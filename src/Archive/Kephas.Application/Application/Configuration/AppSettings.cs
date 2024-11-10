// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Configuration
{
    using Kephas.Configuration;
    using Kephas.Dynamic;

    /// <summary>
    /// Settings for the application instances.
    /// </summary>
    public class AppSettings : Expando, ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether application instances should be automatically started.
        /// </summary>
        public bool AutoStart { get; set; } = true;

        /// <summary>
        /// Get or sets the application startup args.
        /// </summary>
        public Expando? Args { get; set; }

        /// <summary>
        /// Gets or sets the environment variables to set for the child process.
        /// </summary>
        public Expando? EnvironmentVariables { get; set; }

        /// <summary>
        /// Gets or sets the features which are enabled by the application instance.
        /// </summary>
        /// <remarks>
        /// Required features are enabled by default and cannot be disabled.
        /// Also, if a feature is enabled, all dependencies are enabled, too.
        /// </remarks>
        public string[]? EnabledFeatures { get; set; }

        /// <summary>
        /// Gets or sets the commands to be executed upon startup, when the application is started for the first time.
        /// </summary>
        /// <remarks>
        /// The application will take care to remove the executed commands from this list once they were executed.
        /// </remarks>
        public object[]? SetupCommands { get; set; }

        /// <summary>
        /// Gets or sets the commands to be executed upon startup, each time the application is started.
        /// </summary>
        public object[]? StartupCommands { get; set; }

        /// <summary>
        /// Gets or sets the commands to be executed upon shutdown, each time the application is stopped.
        /// </summary>
        public object[]? ShutdownCommands { get; set; }

        /// <summary>
        /// Gets or sets the settings for hosting.
        /// </summary>
        public HostSettings? Host { get; set; }
    }
}