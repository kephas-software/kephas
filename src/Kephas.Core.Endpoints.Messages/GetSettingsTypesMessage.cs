// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSettingsTypesMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Messaging;
    using Kephas.Security.Permissions;
    using Kephas.Security.Permissions.AttributedModel;

    /// <summary>
    /// Message for getting the setting types.
    /// </summary>
    [Display(Description = "Gets the registered settings types.")]
    [RequiresPermission(typeof(AppAdminPermission))]
    public class GetSettingsTypesMessage : IMessage
    {
    }
}