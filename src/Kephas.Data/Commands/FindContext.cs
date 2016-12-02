// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default implementation of a find operationContext.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The default implementation of a find operationContext.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class FindContext<T> : DataOperationContext, IFindContext<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindContext{T}"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="id">The identifier of the entity.</param>
        /// <param name="throwIfNotFound"><c>true</c> to throw an exception if an entity is not found, otherwise <c>false</c>.</param>
        /// <param name="ambientServices">The ambient services (optional). If not provided, <see cref="AmbientServices.Instance"/> will be considered.</param>
        public FindContext(IDataContext dataContext, Id id, bool throwIfNotFound, IAmbientServices ambientServices = null)
            : base(dataContext, ambientServices)
        {
            Contract.Requires(dataContext != null);

            this.Id = id;
            this.ThrowIfNotFound = throwIfNotFound;
            this.EntityType = typeof(T);
        }

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

        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public Type EntityType { get; set; }
    }
}