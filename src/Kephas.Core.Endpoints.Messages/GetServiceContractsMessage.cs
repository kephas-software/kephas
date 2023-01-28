// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetServiceContractsMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Messaging;
    using Kephas.Security.Permissions;
    using Kephas.Security.Permissions.AttributedModel;

    /// <summary>
    /// Message for retrieving metadata of the application service contracts.
    /// </summary>
    [Display(Description = "Gets the metadata of the application service contracts.")]
    [RequiresPermission(typeof(AppAdminPermission))]
    public class GetServiceContractsMessage : IMessage<GetServiceContractsResponse>
    {
        /// <summary>
        /// Gets or sets the contract type.
        /// </summary>
        [Display(Description = "Filter on the contract type.")]
        public string? ContractType { get; set; }

        /// <summary>
        /// Gets or sets the flag for allowing multiple registrations.
        /// </summary>
        [Display(Description = "Filter on the AllowMultiple flag.")]
        public bool? AllowMultiple { get; set; }

        /// <summary>
        /// Gets or sets the flag for the open generic registration.
        /// </summary>
        [Display(Description = "Filter on the AsOpenGeneric flag.")]
        public bool? AsOpenGeneric { get; set; }
    }
}