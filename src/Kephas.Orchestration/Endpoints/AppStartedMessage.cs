// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppStartedMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application started message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Endpoints
{
    /// <summary>
    /// An application started message.
    /// </summary>
    public class AppStartedMessage : IAppMessage
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