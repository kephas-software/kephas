// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetServicesResponse.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using Kephas.Messaging.Messages;
    using Kephas.Services;

    /// <summary>
    /// The response message for <see cref="GetServicesMessage"/>.
    /// </summary>
    public class GetServicesResponse : Response
    {
        /// <summary>
        /// Gets or sets the services.
        /// </summary>
        public AppServiceMetadata[]? Services { get; set; }
    }
}