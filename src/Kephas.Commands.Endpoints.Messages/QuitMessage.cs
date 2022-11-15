// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuitMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the quit message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Endpoints
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Messaging;
    using Kephas.Security.Permissions;
    using Kephas.Security.Permissions.AttributedModel;

    /// <summary>
    /// A quit message.
    /// </summary>
    [Display(Description = "Quits the application.")]
    [RequiresPermission(typeof(AppAdminPermission))]
    public class QuitMessage : IMessage
    {
    }
}
