// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBulkUpdateContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IBulkUpdateContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    /// <summary>
    /// Context for bulk update operation.
    /// </summary>
    public interface IBulkUpdateContext : IBulkDataOperationContext
    {
        /// <summary>
        /// Gets or sets the values to update.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        object Values { get; set; }
    }

    /// <summary>
    /// Generic interface for data operation contexts of the <see cref="IBulkUpdateCommand"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public interface IBulkUpdateContext<TEntity> : IBulkDataOperationContext<TEntity>, IBulkUpdateContext
    {
    }
}