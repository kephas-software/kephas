// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestartMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Endpoints
{
    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;
    using Kephas.Security.Authorization;
    using Kephas.Security.Permissions;
    using Kephas.Security.Permissions.AttributedModel;

    /// <summary>
    /// Restarts the worker application instances.
    /// </summary>
    [DisplayInfo(Description = "Restarts the worker application instances.")]
    [RequiresPermission(typeof(AppAdminPermission))]
    public class RestartMessage : IMessage
    {
    }
}