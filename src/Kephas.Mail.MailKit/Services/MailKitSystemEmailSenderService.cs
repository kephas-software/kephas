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
    using Kephas.Services;

    /// <summary>
    /// A MailKit system email sender service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class MailKitSystemEmailSenderService: MailKitEmailSenderService, ISystemEmailSenderService
    {
    }
}