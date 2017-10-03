// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Endpoint.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the endpoint class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    /// <summary>
    /// A messaging endpoint.
    /// </summary>
    public class Endpoint : IEndpoint
    {
        /// <summary>
        /// Gets or sets the identifier of the endpoint.
        /// </summary>
        /// <value>
        /// The identifier of the endpoint.
        /// </value>
        public string EndpointId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the application instance.
        /// </summary>
        /// <value>
        /// The identifier of the application instance.
        /// </value>
        public string AppInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the application.
        /// </summary>
        /// <value>
        /// The identifier of the application.
        /// </value>
        public string AppId { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(this.EndpointId)
                       ? string.IsNullOrEmpty(this.AppInstanceId)
                             ? this.AppId
                             : this.AppInstanceId
                       : this.EndpointId;
        }
    }
}