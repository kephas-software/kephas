// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BulkDeleteContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the purge context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Context for the bulk delete operation.
    /// </summary>
    public class BulkDeleteContext : BulkDataOperationContext, IBulkDeleteContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkDeleteContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="criteria">The criteria for finding the entities to delete.</param>
        /// <param name="throwIfNotFound">Optional. <c>true</c> to throw an exception if an entity is not found, otherwise <c>false</c>.</param>
        public BulkDeleteContext(IDataContext dataContext, Type entityType, Expression criteria, bool throwIfNotFound = false)
            : base(dataContext, entityType, criteria, throwIfNotFound)
        {
        }
    }

    /// <summary>
    /// Generic context for the bulk delete operation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class BulkDeleteContext<T> : BulkDataOperationContext<T>, IBulkDeleteContext<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkDeleteContext{T}"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="criteria">The criteria for finding the entities to delete.</param>
        /// <param name="throwIfNotFound">Optional. <c>true</c> to throw an exception if an entity is not found, otherwise <c>false</c>.</param>
        public BulkDeleteContext(IDataContext dataContext, Expression<Func<T, bool>> criteria, bool throwIfNotFound = false)
            : base(dataContext, criteria, throwIfNotFound)
        {
        }
    }
}