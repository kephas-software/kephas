// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the query message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Endpoints
{
    using Kephas.Data.Client.Queries;
    using Kephas.Messaging;

    /// <summary>
    /// A query message.
    /// </summary>
    public class QueryMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the query to be executed.
        /// </summary>
        /// <value>
        /// The query to be executed.
        /// </value>
        public ClientQuery Query { get; set; }
    }

    /// <summary>
    /// A query response message.
    /// </summary>
    public class QueryResponseMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the entities.
        /// </summary>
        /// <value>
        /// The entities.
        /// </value>
        public object[] Entities { get; set; }
    }
}