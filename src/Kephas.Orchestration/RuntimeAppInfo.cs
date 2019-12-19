// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration
{
    using System.Collections.Generic;

    using Kephas.Dynamic;

    /// <summary>
    /// Runtime information about the application.
    /// </summary>
    public class RuntimeAppInfo : IRuntimeAppInfo
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

        /// <summary>
        /// Gets or sets the application properties.
        /// </summary>
        /// <value>
        /// The application properties.
        /// </value>
        public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return this.AppInstanceId?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// True if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is RuntimeAppInfo info)
            {
                return info.AppInstanceId == this.AppInstanceId;
            }

            return false;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.AppId}/{this.AppInstanceId} on {this.HostName} ({this.HostAddress}), pid:{this.ProcessId}";
        }
    }
}