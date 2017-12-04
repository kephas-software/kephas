// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRef.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Structure used to define and retrieve a referenced entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Contract used to define and retrieve a referenced entity.
    /// </summary>
    public interface IRef : IIdentifiable
    {
        /// <summary>
        /// Gets or sets the identifier of the referenced entity.
        /// </summary>
        /// <value>
        /// The identifier of the referenced entity.
        /// </value>
        new object Id { get; set; }

        /// <summary>
        /// Gets the type of the referenced entity.
        /// </summary>
        /// <value>
        /// The type of the referenced entity.
        /// </value>
        Type EntityType { get; }

        /// <summary>
        /// Gets a value indicating whether the reference is empty/not set.
        /// </summary>
        /// <value>
        /// True if this reference is empty, false if not.
        /// </value>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets the referenced entity asynchronously.
        /// </summary>
        /// <param name="throwIfNotFound">If true and the referenced entity is not found, an exception occurs.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A task promising the referenced entity.
        /// </returns>
        Task<object> GetAsync(bool throwIfNotFound = true, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Generic contract used to define and retrieve a referenced entity.
    /// </summary>
    /// <typeparam name="T">The referenced entity type.</typeparam>
    public interface IRef<T> : IRef
        where T : class
    {
        /// <summary>
        /// Gets the referenced entity asynchronously.
        /// </summary>
        /// <param name="throwIfNotFound">If true and the referenced entity is not found, an exception occurs.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A task promising the referenced entity.
        /// </returns>
        new Task<T> GetAsync(bool throwIfNotFound = true, CancellationToken cancellationToken = default);
    }
}