// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopAppMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the stop application message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Endpoints
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Security.Permissions;
    using Kephas.Security.Permissions.AttributedModel;

    /// <summary>
    /// A stop application message.
    /// </summary>
    [Display(Description = "Stops a worker application instance.")]
    [RequiresPermission(typeof(AppAdminPermission))]
    public class StopAppMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the identifier of the application.
        /// </summary>
        /// <value>
        /// The identifier of the application.
        /// </value>
        [Display(Description = "Optional. The ID of the app to be stopped.")]
        public string? AppId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the application instance.
        /// </summary>
        /// <value>
        /// The identifier of the application instance.
        /// </value>
        [Display(Description = "Optional. The ID of the app instance to be stopped.")]
        public string? AppInstanceId { get; set; }
    }

    /// <summary>
    /// A stop application response message.
    /// </summary>
    public class StopAppResponse : Response
    {
        /// <summary>
        /// Gets or sets the identifier of the process.
        /// </summary>
        /// <value>
        /// The identifier of the process.
        /// </value>
        public int ProcessId { get; set; }
    }
}
