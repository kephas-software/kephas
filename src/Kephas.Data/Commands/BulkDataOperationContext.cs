// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BulkDataOperationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the bulk data operation context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Linq.Expressions;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Context for bulk operations.
    /// </summary>
    public class BulkDataOperationContext : DataOperationContext, IBulkDataOperationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkDataOperationContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="criteria">The criteria for finding the entities to operate on.</param>
        /// <param name="throwIfNotFound">Optional. <c>true</c> to throw an exception if no entities are affected, otherwise <c>false</c>.</param>
        public BulkDataOperationContext(
            IDataContext dataContext,
            Type entityType,
            Expression criteria,
            bool throwIfNotFound = false)
            : base(dataContext)
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(entityType, nameof(entityType));
            Requires.NotNull(criteria, nameof(criteria));

            this.EntityType = entityType;
            this.Criteria = criteria;
            this.ThrowOnNotFound = throwIfNotFound;
        }

        /// <summary>
        /// Gets or sets the criteria for finding the entities to operate on.
        /// </summary>
        /// <value>
        /// The criteria for finding the entities to operate on.
        /// </value>
        public Expression Criteria { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to throw an exception if there were no entities to operate on.
        /// </summary>
        /// <value>
        /// <c>true</c>true to throw an exception if there were no entities to operate on, otherwise <c>false</c>.
        /// </value>
        public bool ThrowOnNotFound { get; set; }

        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public Type EntityType { get; set; }
    }

    /// <summary>
    /// Generic context for bulk operations.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class BulkDataOperationContext<T> : BulkDataOperationContext, IBulkDataOperationContext<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkDataOperationContext{T}"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="criteria">The criteria for finding the entities to operate on.</param>
        /// <param name="throwIfNotFound">Optional. <c>true</c> to throw an exception if no entities are affected, otherwise <c>false</c>.</param>
        public BulkDataOperationContext(
            IDataContext dataContext,
            Expression<Func<T, bool>> criteria,
            bool throwIfNotFound = false)
            : base(dataContext, typeof(T), criteria, throwIfNotFound)
        {
            Requires.NotNull(dataContext, nameof(dataContext));
        }

        /// <summary>
        /// Gets or sets the criteria for finding the entities to operate on.
        /// </summary>
        /// <value>
        /// The criteria for finding the entities to operate on.
        /// </value>
        public new Expression<Func<T, bool>> Criteria
        {
            get => (Expression<Func<T, bool>>)base.Criteria;
            set => base.Criteria = value;
        }
    }
}