// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataResponse.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataResponse interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Endpoints
{
    using System.Collections.Generic;

    using Kephas.Data.Client.Capabilities;
    using Kephas.Messaging;

    /// <summary>
    /// Contract for responses returning updated data (to be used for refreshing the client cache, for example).
    /// </summary>
    public interface IDataResponse
    {
        /// <summary>
        /// Gets or sets the entity entries after a data operation.
        /// </summary>
        /// <value>
        /// The entity entries after a data operation.
        /// </value>
        IList<DtoEntityEntry> EntityEntries { get; set; }
    }
}