// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEndpoint.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEndpoint interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;

    /// <summary>
    /// Interface for messaging endpoint.
    /// </summary>
    public interface IEndpoint
    {
        /// <summary>
        /// Gets the URL of the endpoint.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        Uri Url { get; }

        /// <summary>
        /// Gets the identifier of the endpoint.
        /// </summary>
        /// <value>
        /// The identifier of the endpoint.
        /// </value>
        string EndpointId { get; }

        /// <summary>
        /// Gets the identifier of the application instance.
        /// </summary>
        /// <value>
        /// The identifier of the application instance.
        /// </value>
        string AppInstanceId { get; }

        /// <summary>
        /// Gets the identifier of the application.
        /// </summary>
        /// <value>
        /// The identifier of the application.
        /// </value>
        string AppId { get; }
    }
}