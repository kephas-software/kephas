﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientQuery.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the client query class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries
{
    /// <summary>
    /// A client query.
    /// </summary>
    public class ClientQuery
    {
        /// <summary>
        /// Gets or sets the selected entity type.
        /// </summary>
        /// <value>
        /// The selected entity type.
        /// </value>
        public string EntityType { get; set; }

        /// <summary>
        /// Gets or sets the filter condition.
        /// </summary>
        /// <value>
        /// The filter condition.
        /// </value>
        public Expression Filter { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public Expression Order { get; set; }

        /// <summary>
        /// Gets or sets the number of records to skip.
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// Gets or sets the number of records to take.
        /// </summary>
        public int? Take { get; set; }
    }
}