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
    public class MailKitSystemEmailSenderService : MailKitEmailSenderServiceBase, ISystemEmailSenderService
    {
        /// <summary>
        /// The system email sender configuration.
        /// </summary>
        private readonly IConfiguration<SystemSmtpSettings> systemEmailSenderConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitSystemEmailSenderService"/> class.
        /// </summary>
        /// <param name="systemEmailSenderConfig">The system email sender configuration.</param>
        public MailKitSystemEmailSenderService(IConfiguration<SystemSmtpSettings> systemEmailSenderConfig)
        {
            Requires.NotNull(systemEmailSenderConfig, nameof(systemEmailSenderConfig));

            this.systemEmailSenderConfig = systemEmailSenderConfig;
        }

        /// <summary>
        /// Gets the email sender settings.
        /// </summary>
        /// <returns>
        /// The email sender settings.
        /// </returns>
        protected override SmtpSettings GetEmailSenderSettings() => this.systemEmailSenderConfig.Settings;
    }
}