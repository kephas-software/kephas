// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistChangesMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the persist changes message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Endpoints
{
    using System.Collections.Generic;

    using Kephas.Data.Client.Capabilities;
    using Kephas.Messaging;

    /// <summary>
    /// A persist changes message.
    /// </summary>
    public class PersistChangesMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the entity infos to persist.
        /// </summary>
        /// <value>
        /// The entity infos to persist.
        /// </value>
        public IList<DtoEntityInfo> EntityInfos { get; set; }
    }

    /// <summary>
    /// A persist changes response message.
    /// </summary>
    public class PersistChangesResponseMessage : DataResponseMessage
    {
    }
}