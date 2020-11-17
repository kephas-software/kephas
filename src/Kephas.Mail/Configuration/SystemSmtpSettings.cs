﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemSmtpSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the system email sender settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Configuration
{
    using Kephas.Configuration;
    using Kephas.Security.Authorization;
    using Kephas.Security.Authorization.AttributedModel;

    /// <summary>
    /// A system email sender settings.
    /// </summary>
    [RequiresPermission(typeof(AppAdminPermission))]
    public class SystemSmtpSettings : SmtpSettings, ISettings
    {
    }
}