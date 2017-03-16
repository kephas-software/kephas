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
        /// Gets or sets the selected item type.
        /// </summary>
        /// <value>
        /// The selected item type.
        /// </value>
        public string ItemType { get; set; }

        /// <summary>
        /// Gets or sets the where condition.
        /// </summary>
        /// <value>
        /// The where condition.
        /// </value>
        public Expression Where { get; set; }
    }
}