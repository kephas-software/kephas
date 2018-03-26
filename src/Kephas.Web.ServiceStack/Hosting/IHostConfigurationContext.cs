// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHostConfigurationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IHostConfigurationContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.ServiceStack.Hosting
{
    using Kephas.Services;

    using global::ServiceStack;

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