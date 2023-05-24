// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetLiveAppsMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Endpoints
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Security.Permissions;
    using Kephas.Security.Permissions.AttributedModel;

    /// <summary>
    /// Message for getting the live application instances.
    /// </summary>
    [Display(Description = "Gets the live application instances.")]
    [RequiresPermission(typeof(AppAdminPermission))]
    public class GetLiveAppsMessage : IMessage<GetLiveAppsResponse>
    {
    }

    /// <summary>
    /// Response message for <see cref="GetLiveAppsMessage"/>.
    /// </summary>
    public class GetLiveAppsResponse : Response
    {
        /// <summary>
        /// Gets or sets the live apps.
        /// </summary>
        public IRuntimeAppInfo[] Apps { get; set; }
    }
}