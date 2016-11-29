// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default implementation of a find operationContext.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kephas.Data.Commands
{
    using System.Diagnostics.Contracts;

    public class FindContext : DataOperationContext, IFindContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="id">The identifier of the entity.</param>
        /// <param name="throwIfNotFound"><c>true</c> to throw an exception if an entity is not found,
        ///                               otherwise <c>false</c>.</param>
        public FindContext(IDataContext dataContext, Type entityType, Id id, bool throwIfNotFound)
            : base(dataContext)
        {
            Contract.Requires(dataContext != null);
            Contract.Requires(entityType != null);

            this.EntityType = entityType;
            this.Id = id;
            this.ThrowIfNotFound = throwIfNotFound;
        }

        public Type EntityType { get; }

        /// <summary>
        /// Gets or sets the identifier of the entity to find.
        /// </summary>
        /// <value>
        /// The identifier of the entity.
        /// </value>
        public Id Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to throw an exception if an entity is not found.
        /// </summary>
        /// <value>
        /// <c>true</c>true to throw an exception if an entity is not found, otherwise <c>false</c>.
        /// </value>
        public bool ThrowIfNotFound { get; set; }
    }

    /// <summary>
    /// The default implementation of a find operationContext.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class FindContext<T> : FindContext, IFindContext<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindContext{T}"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="id">The identifier of the entity.</param>
        /// <param name="throwIfNotFound"><c>
        ///                               true</c>
        ///                               to throw an exception if an entity is not found,
        ///                               otherwise <c>
        ///                               false</c>
        ///                               .</param>
        public FindContext(IDataContext dataContext, Id id, bool throwIfNotFound)
            : base(dataContext, typeof(T), id, throwIfNotFound)
        {
            Contract.Requires(dataContext != null);
        }
    }
}