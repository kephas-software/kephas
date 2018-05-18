// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppStoppedMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application stopped message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Endpoints
{
    using Kephas.Messaging;

    /// <summary>
    /// An application stopped message.
    /// </summary>
    public class AppStoppedMessage : IAppMessage
    {
        /// <summary>
        /// Gets or sets information describing the application.
        /// </summary>
        /// <value>
        /// Information describing the application.
        /// </value>
        public IAppInfo AppInfo { get; set; }
    }
}