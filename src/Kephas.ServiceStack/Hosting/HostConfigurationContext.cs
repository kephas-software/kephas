// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostConfigurationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the host configuration context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ServiceStack.Hosting
{
    using global::ServiceStack;

    using Kephas.Services;

    /// <summary>
    /// A host configuration context.
    /// </summary>
    public class HostConfigurationContext : Context, IHostConfigurationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HostConfigurationContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="host">The host.</param>
        /// <param name="hostConfig">The host configuration.</param>
        public HostConfigurationContext(IAmbientServices ambientServices, ServiceStackHost host, HostConfig hostConfig)
            : base(ambientServices)
        {
            this.Host = host;
            this.HostConfig = hostConfig;
        }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        public ServiceStackHost Host { get; }

        /// <summary>
        /// Gets the host configuration.
        /// </summary>
        /// <value>
        /// The host configuration.
        /// </value>
        public HostConfig HostConfig { get; }

        /// <summary>
        /// Gets or sets the base urls.
        /// </summary>
        /// <value>
        /// The base urls.
        /// </value>
        public string[] BaseUrls { get; set; }
    }
}