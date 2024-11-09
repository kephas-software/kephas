// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBulkDataOperationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IBulkDataOperationContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Interface for bulk data operation context.
    /// </summary>
    public interface IBulkDataOperationContext : IDataOperationContext
    {
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        Type EntityType { get; }

        /// <summary>
        /// Gets a value indicating whether to throw an exception if there were no entities to operate on.
        /// </summary>
        /// <value>
        /// <c>true</c>true to throw an exception if there were no entities to operate on, otherwise <c>false</c>.
        /// </value>
        bool ThrowOnNotFound { get; }

        /// <summary>
        /// Gets the criteria of the entities to operate on.
        /// </summary>
        /// <value>
        /// The criteria of the entities to operate on.
        /// </value>
        Expression Criteria { get; }
    }

    /// <summary>
    /// Generic interface for contexts used in bulk data operations.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public interface IBulkDataOperationContext<TEntity> : IBulkDataOperationContext
    {
        /// <summary>
        /// Gets the criteria of the entities to purge.
        /// </summary>
        /// <remarks>
        /// Overrides the untyped expression from the base interface
        /// to provide LINQ-support.
        /// </remarks>
        /// <value>
        /// The criteria of the entities to purge.
        /// </value>
        new Expression<Func<TEntity, bool>> Criteria { get; }
    }
}