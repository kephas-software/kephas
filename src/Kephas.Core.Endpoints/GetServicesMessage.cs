// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetServicesMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Services.Composition;

    /// <summary>
    /// Message for retrieving metadata of the application services.
    /// </summary>
    [DisplayInfo(Description = "Gets the metadata of the application services.")]
    public class GetServicesMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the contract type.
        /// </summary>
        [DisplayInfo(Description = "Filter on the contract type.")]
        public string? ContractType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the ordering behavior should be applied on the services.
        /// </summary>
        [DisplayInfo(Description = "Indicates whether the ordering behavior should be applied on the services.")]
        public bool Ordered { get; set; }
    }

    /// <summary>
    /// The response message for <see cref="GetServicesMessage"/>.
    /// </summary>
    public class GetServicesResponseMessage : ResponseMessage
    {
        /// <summary>
        /// Gets or sets the services.
        /// </summary>
        public AppServiceMetadata[]? Services { get; set; }
    }
}