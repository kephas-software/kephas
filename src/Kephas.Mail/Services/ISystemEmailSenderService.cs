﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISystemEmailSenderService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
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
    /// Singleton application service for sending system emails.
    /// </summary>
    [SingletonAppServiceContract]
    public interface ISystemEmailSenderService : IEmailSenderService
    {
    }
}