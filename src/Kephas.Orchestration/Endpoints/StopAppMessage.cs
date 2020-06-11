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
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Security.Authorization;
    using Kephas.Security.Authorization.AttributedModel;

    /// <summary>
    /// A stop application message.
    /// </summary>
    [RequiresPermission(typeof(AppAdminPermission))]
    public class StopAppMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the identifier of the application.
        /// </summary>
        /// <value>
        /// The identifier of the application.
        /// </value>
        public string? AppId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the application instance.
        /// </summary>
        /// <value>
        /// The identifier of the application instance.
        /// </value>
        public string? AppInstanceId { get; set; }
    }

    /// <summary>
    /// A stop application response message.
    /// </summary>
    public class StopAppResponseMessage : ResponseMessage
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
