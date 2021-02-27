// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetServiceContractsMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Message for retrieving metadata of the application service contracts.
    /// </summary>
    [DisplayInfo(Description = "Gets the metadata of the application service contracts.")]
    public class GetServiceContractsMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the contract type.
        /// </summary>
        [DisplayInfo(Description = "Filter on the contract type.")]
        public string? ContractType { get; set; }

        /// <summary>
        /// Gets or sets the flag for allowing multiple registrations.
        /// </summary>
        [DisplayInfo(Description = "Filter on the AllowMultiple flag.")]
        public bool? AllowMultiple { get; set; }

        /// <summary>
        /// Gets or sets the flag for the open generic registration.
        /// </summary>
        [DisplayInfo(Description = "Filter on the AsOpenGeneric flag.")]
        public bool? AsOpenGeneric { get; set; }
    }

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