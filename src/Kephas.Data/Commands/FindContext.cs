// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default implementation of a find context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The default implementation of a find context.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class FindContext<T> : DataContext, IFindContext<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindContext{T}"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="id">The identifier of the entity.</param>
        /// <param name="throwIfNotFound"><c>
        ///                               true</c>
        ///                               true to throw an exception if an entity is not found,
        ///                               otherwise <c>
        ///                               false</c>
        ///                               .</param>
        public FindContext(IDataRepository repository, Id id, bool throwIfNotFound)
            : base(repository)
        {
            Contract.Requires(repository != null);

            this.Id = id;
            this.ThrowIfNotFound = throwIfNotFound;
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
    }
}