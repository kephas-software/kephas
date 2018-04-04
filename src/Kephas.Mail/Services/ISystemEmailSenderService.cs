// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISystemEmailSenderService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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