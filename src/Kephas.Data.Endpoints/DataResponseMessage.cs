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

    /// <summary>
    /// A data response message.
    /// </summary>
    public class DataResponseMessage : IDataResponseMessage
    {
        /// <summary>
        /// Gets or sets the entity infos after a data operation.
        /// </summary>
        /// <value>
        /// The entity infos after a data operation.
        /// </value>
        public IList<ClientEntityInfo> EntityInfos { get; set; }
    }
}