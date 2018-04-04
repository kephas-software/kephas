// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHostConfigurationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IHostConfigurationContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ServiceStack.Hosting
{
    using global::ServiceStack;

    using Kephas.Services;

    /// <summary>
    /// Context used in host configuration.
    /// </summary>
    public interface IHostConfigurationContext : IContext
    {
        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        ServiceStackHost Host { get; }

        /// <summary>
        /// Gets the host configuration.
        /// </summary>
        /// <value>
        /// The host configuration.
        /// </value>
        HostConfig HostConfig { get; }

        /// <summary>
        /// Gets or sets the base urls.
        /// </summary>
        /// <value>
        /// The base urls.
        /// </value>
        string[] BaseUrls { get; set; }
    }
}