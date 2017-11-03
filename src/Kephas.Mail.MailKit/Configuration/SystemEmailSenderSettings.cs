// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemEmailSenderSettings.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the system email sender settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Configuration
{
    using Kephas.Cryptography;

    /// <summary>
    /// A system email sender settings.
    /// </summary>
    public class SystemEmailSenderSettings
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Encrypted]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public string Port { get; set; }
    }
}