// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataResponseMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data response message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Endpoints
{
    using System.Collections.Generic;

    using Kephas.Data.Client.Capabilities;
    using Kephas.Messaging.Messages;

    /// <summary>
    /// A data response message.
    /// </summary>
    public class DataResponseMessage : ResponseMessage, IDataResponseMessage
    {
        /// <summary>
        /// Gets or sets the entity entries after a data operation.
        /// </summary>
        /// <value>
        /// The entity entries after a data operation.
        /// </value>
        public IList<DtoEntityEntry> EntityEntries { get; set; } = new List<DtoEntityEntry>();
    }
}