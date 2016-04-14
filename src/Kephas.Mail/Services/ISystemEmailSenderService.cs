// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISystemEmailSenderService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the ISystemEmailSenderService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Services
{
    using Kephas.Services;

    /// <summary>
    /// Shared application service for sending system emails.
    /// </summary>
    [SharedAppServiceContract]
    public interface ISystemEmailSenderService : IEmailSenderService
    {
    }
}