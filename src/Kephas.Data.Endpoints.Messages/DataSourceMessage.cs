// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSourceMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data source message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Endpoints
{
    using System.Collections.Generic;

    using Kephas.Messaging;

    /// <summary>
    /// Message for retrieving a data source.
    /// </summary>
    public class DataSourceMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public string EntityType { get; set; }

        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        public string Property { get; set; }

        /// <summary>
        /// Gets or sets options for controlling the operation.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public object Options { get; set; }
    }

    /// <summary>
    /// A data source response message.
    /// </summary>
    public class DataSourceResponseMessage : IResponse
    {
        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        /// <value>
        /// The data source.
        /// </value>
        public IList<object>? DataSource { get; set; }
    }
}