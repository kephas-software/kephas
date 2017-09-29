// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEndpoint.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IEndpoint interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    /// <summary>
    /// Interface for messaging endpoint.
    /// </summary>
    public interface IEndpoint
    {
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