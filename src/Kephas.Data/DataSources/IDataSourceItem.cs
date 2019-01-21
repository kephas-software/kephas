// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataSourceItem.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataSourceItem interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.DataSources
{
    /// <summary>
    /// Interface for data source item.
    /// </summary>
    public interface IDataSourceItem : IIdentifiable
    {
        /// <summary>
        /// Gets or sets the display text to be shown in the editors.
        /// </summary>
        /// <value>
        /// The display text.
        /// </value>
        string DisplayText { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the parent.
        /// </summary>
        /// <value>
        /// The identifier of the parent.
        /// </value>
        object ParentId { get; set; }
    }
}