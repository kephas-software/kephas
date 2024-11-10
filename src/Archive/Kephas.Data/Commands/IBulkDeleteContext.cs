// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBulkDeleteContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IBulkDeleteContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    /// <summary>
    /// Context for bulk delete operation.
    /// </summary>
    public interface IBulkDeleteContext : IBulkDataOperationContext
    {
    }

    /// <summary>
    /// Generic interface for data operation contexts of the <see cref="IBulkDeleteCommand"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public interface IBulkDeleteContext<TEntity> : IBulkDataOperationContext<TEntity>, IBulkDeleteContext
    {
    }
}