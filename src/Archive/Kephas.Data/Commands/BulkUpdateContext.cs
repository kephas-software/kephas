// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BulkUpdateContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the bulk update context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Context for the bulk update operation.
    /// </summary>
    public class BulkUpdateContext : BulkDataOperationContext, IBulkUpdateContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkUpdateContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="criteria">The criteria for finding the entities to delete.</param>
        /// <param name="values">The values.</param>
        /// <param name="throwIfNotFound">Optional. <c>true</c> to throw an exception if an entity is not found, otherwise <c>false</c>.</param>
        public BulkUpdateContext(IDataContext dataContext, Type entityType, Expression criteria, object values, bool throwIfNotFound = false)
            : base(dataContext, entityType, criteria, throwIfNotFound)
        {
            this.Values = values;
        }

        /// <summary>
        /// Gets or sets the values to update.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public object Values { get; set; }
    }

    /// <summary>
    /// Generic context for the bulk update operation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class BulkUpdateContext<T> : BulkDataOperationContext<T>, IBulkUpdateContext<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkUpdateContext{T}"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="criteria">The criteria for finding the entities to delete.</param>
        /// <param name="values">The values.</param>
        /// <param name="throwIfNotFound">Optional. <c>true</c> to throw an exception if an entity is not found, otherwise <c>false</c>.</param>
        public BulkUpdateContext(IDataContext dataContext, Expression<Func<T, bool>> criteria, object values, bool throwIfNotFound = false)
            : base(dataContext, criteria, throwIfNotFound)
        {
            this.Values = values;
        }

        /// <summary>
        /// Gets or sets the values to update.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public object Values { get; set; }
    }
}