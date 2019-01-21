// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSourceItem.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data source item class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.DataSources
{
    /// <summary>
    /// A data source item.
    /// </summary>
    public class DataSourceItem : IDataSourceItem
    {
        /// <summary>
        /// Identifier for the identifiable.
        /// </summary>
        object IIdentifiable.Id => this.Id;

        /// <summary>
        /// Gets or sets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public object Id { get; set; }

        /// <summary>
        /// Gets or sets the display text to be shown in the editors.
        /// </summary>
        /// <value>
        /// The display text.
        /// </value>
        public string DisplayText { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the parent.
        /// </summary>
        /// <value>
        /// The identifier of the parent.
        /// </value>
        public object ParentId { get; set; }
    }
}