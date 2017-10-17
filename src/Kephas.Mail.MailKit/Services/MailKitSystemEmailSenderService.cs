// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailKitSystemEmailSenderService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the mail kit system email sender service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Services
{
    using System.Net;

    using Kephas.Configuration;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Mail.Configuration;
    using Kephas.Services;

    /// <summary>
    /// A MailKit system email sender service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class MailKitSystemEmailSenderService : MailKitEmailSenderService, ISystemEmailSenderService
    {
        /// <summary>
        /// The system email sender configuration.
        /// </summary>
        private readonly IConfiguration<SystemEmailSenderSettings> systemEmailSenderConfig;

        /// <summary>
        /// Information describing the connection.
        /// </summary>
        private (ICredentials credentials, string host, int port)? connectionData;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitSystemEmailSenderService"/> class.
        /// </summary>
        /// <param name="systemEmailSenderConfig">The system email sender configuration.</param>
        public MailKitSystemEmailSenderService(IConfiguration<SystemEmailSenderSettings> systemEmailSenderConfig)
        {
            Requires.NotNull(systemEmailSenderConfig, nameof(systemEmailSenderConfig));

            this.systemEmailSenderConfig = systemEmailSenderConfig;
        }

        /// <summary>
        /// Gets the connection data.
        /// </summary>
        /// <param name="emailMessage">The email message.</param>
        /// <returns>
        /// The connection data.
        /// </returns>
        protected override (ICredentials credentials, string host, int port) GetConnectionData(IEmailMessage emailMessage)
        {
            return this.connectionData ?? (this.connectionData = this.ComputeConnectionData()).Value;
        }

        /// <summary>
        /// Calculates the connection data.
        /// </summary>
        /// <returns>
        /// The calculated connection data.
        /// </returns>
        private (NetworkCredential, string Host, int) ComputeConnectionData()
        {
            var settings = this.systemEmailSenderConfig.Settings;
            return (new NetworkCredential(settings.UserName, settings.Password), settings.Host, int.Parse(settings.Port));
        }
    }
}