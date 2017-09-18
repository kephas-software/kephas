// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmtpSystemEmailSenderService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the SMTP system email sender service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Services
{
    using Kephas.Services;

    /// <summary>
    /// A SMTP system email sender service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class SmtpSystemEmailSenderService : SmtpEmailSenderService, ISystemEmailSenderService
    {
    }
}