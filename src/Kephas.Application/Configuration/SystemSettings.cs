// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Configuration
{
    using System;
    using System.Collections.Generic;

    using Kephas.Application.Cryptography.X509Certificates;
    using Kephas.Dynamic;

    /// <summary>
    /// Settings for the application runtime.
    /// </summary>
    public class SystemSettings : Expando
    {
        /// <summary>
        /// Gets the settings for the application instances.
        /// </summary>
        public IDictionary<string, AppSettings> Instances { get; } = new Dictionary<string, AppSettings>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Gets the certificate information.
        /// </summary>
        public IDictionary<string, CertificateSettings>? Certificates { get; } = new Dictionary<string, CertificateSettings>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Gets or sets the commands to be executed upon startup, when the application is started for the first time.
        /// </summary>
        /// <remarks>
        /// The application will take care to remove the executed commands from this list once they were executed.
        /// </remarks>
        public object[]? SetupCommands { get; set; }
    }
}
