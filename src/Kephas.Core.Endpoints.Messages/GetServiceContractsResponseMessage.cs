// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetServiceContractsResponseMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using Kephas.Messaging.Messages;
    using Kephas.Services.Reflection;

    /// <summary>
    /// The response message for <see cref="GetServiceContractsMessage"/>.
    /// </summary>
    public class GetServiceContractsResponseMessage : ResponseMessage
    {
        /// <summary>
        /// Gets or sets the service information.
        /// </summary>
        public IAppServiceInfo[]? ServiceInfos { get; set; }
    }
}