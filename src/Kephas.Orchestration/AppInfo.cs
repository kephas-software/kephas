// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration
{
    using Kephas.Dynamic;

    /// <summary>
    /// Information about the application.
    /// </summary>
    public class AppInfo : Expando, IAppInfo
    {
        /// <summary>
        /// Gets or sets the identifier of the application.
        /// </summary>
        /// <value>
        /// The identifier of the application.
        /// </value>
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the application instance.
        /// </summary>
        /// <value>
        /// The identifier of the application instance.
        /// </value>
        public string AppInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the aupported app features.
        /// </summary>
        /// <value>
        /// The supported app features.
        /// </value>
        public string[] Features { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the process running the application.
        /// </summary>
        /// <value>
        /// The identifier of the process.
        /// </value>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the name of the host where the application process runs.
        /// </summary>
        /// <value>
        /// The name of the host.
        /// </value>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the address of the host where the application process runs.
        /// </summary>
        /// <value>
        /// The host address.
        /// </value>
        public string HostAddress { get; set; }
    }
}