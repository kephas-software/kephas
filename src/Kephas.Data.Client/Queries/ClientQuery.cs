// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientQuery.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    }
}