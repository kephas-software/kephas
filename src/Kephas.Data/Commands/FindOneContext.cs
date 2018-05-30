// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindOneContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the find one context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Linq.Expressions;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A find one context.
    /// </summary>
    public class FindOneContext : DataOperationContext, IFindOneContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindOneContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="criteria">The criteria for finding the entity.</param>
        /// <param name="throwIfNotFound"><c>true</c> to throw an exception if an entity is not found, otherwise <c>false</c> (optional).</param>
        public FindOneContext(IDataContext dataContext, Type entityType, Expression criteria, bool throwIfNotFound = true)
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
        /// Gets or sets the criteria for finding the entity.
        /// </summary>
        /// <value>
        /// The criteria for finding the entity.
        /// </value>
        public Expression Criteria { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to throw an exception if an entity is not found.
        /// </summary>
        /// <value>
        /// <c>true</c>true to throw an exception if an entity is not found, otherwise <c>false</c>.
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
    /// The default implementation of a find operationContext.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class FindOneContext<T> : FindOneContext, IFindOneContext<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindOneContext{T}"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="criteria">The criteria for finding the entity.</param>
        /// <param name="throwIfNotFound"><c>true</c> to throw an exception if an entity is not found, otherwise <c>false</c> (optional).</param>
        public FindOneContext(IDataContext dataContext, Expression<Func<T, bool>> criteria, bool throwIfNotFound = true)
            : base(dataContext, typeof(T), criteria, throwIfNotFound)
        {
            Requires.NotNull(dataContext, nameof(dataContext));
        }

        /// <summary>
        /// Gets or sets the criteria for finding the entity.
        /// </summary>
        /// <value>
        /// The criteria for finding the entity.
        /// </value>
        public new Expression<Func<T, bool>> Criteria
        {
            get => (Expression<Func<T, bool>>)base.Criteria;
            set => base.Criteria = value;
        }
    }
}