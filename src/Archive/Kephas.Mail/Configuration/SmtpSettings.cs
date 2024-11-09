// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmtpSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the email sender settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Configuration
{
    using Kephas.Cryptography;

    /// <summary>
    /// A email sender settings.
    /// </summary>
    public class SmtpSettings
    {
        /// <summary>
        /// The default SMTP port.
        /// </summary>
        public const int DefaultSmtpPort = 25;

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Encrypted]
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        public string? Host { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int? Port { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Gets or sets the name to display.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the ssl accept all certificates.
        /// </summary>
        /// <value>
        /// True if ssl accept all certificates, false if not.
        /// </value>
        public bool SslAcceptAllCertificates { get; set; }

        /// <summary>
        /// Gets or sets the ssl allowed suites.
        /// </summary>
        /// <value>
        /// The ssl allowed suites.
        /// </value>
        public string? SslAllowedSuites { get; set; }

        /// <summary>
        /// Gets or sets the ssl allowed versions.
        /// </summary>
        /// <value>
        /// The ssl allowed versions.
        /// </value>
        public string? SslAllowedVersions { get; set; }
    }
}