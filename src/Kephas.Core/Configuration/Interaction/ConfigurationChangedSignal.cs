// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationChangedSignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration.Interaction
{
    using System;

    using Kephas.ExceptionHandling;
    using Kephas.Interaction;

    /// <summary>
    /// Signal for configuration change.
    /// </summary>
    public class ConfigurationChangedSignal : SignalBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationChangedSignal"/> class.
        /// </summary>
        public ConfigurationChangedSignal()
            : this("Configuration changed.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationChangedSignal"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="severity">Optional. The severity.</param>
        public ConfigurationChangedSignal(string message, SeverityLevel severity = SeverityLevel.Info)
            : base(message, severity)
        {
        }

        /// <summary>
        /// Gets or sets the settings type.
        /// </summary>
        public Type? SettingsType { get; set; }

        /// <summary>
        /// Gets or sets the app instance ID which is source for the change.
        /// </summary>
        public string? SourceAppInstanceId { get; set; }
    }
}