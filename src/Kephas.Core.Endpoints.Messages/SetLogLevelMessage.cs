// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetLogLevelMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the set log level message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Security.Authorization;
    using Kephas.Security.Permissions;
    using Kephas.Security.Permissions.AttributedModel;

    /// <summary>
    /// A set log level message.
    /// </summary>
    [Display(Description = "Sets the application minimum log level.")]
    [RequiresPermission(typeof(AppAdminPermission))]
    public class SetLogLevelMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the minimum level.
        /// </summary>
        /// <value>
        /// The minimum level.
        /// </value>
        [Display(Description = "The minimum log level. Use one of the values: Fatal, Error, Warning, Info, Debug, or Trace.")]
        public LogLevel MinimumLevel { get; set; }
    }
}