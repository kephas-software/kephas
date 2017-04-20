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

    using Kephas.Data.Commands;

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
        new Id Id { get; set; }

        /// <summary>
        /// Gets the type of the referenced entity.
        /// </summary>
        /// <value>
        /// The type of the referenced entity.
        /// </value>
        Type EntityType { get; }

        /// <summary>
        /// Gets the referenced entity asynchronously.
        /// </summary>
        /// <param name="operationContext">The context for finding the entity (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A task promising the referenced entity.
        /// </returns>
        Task<object> GetAsync(IFindContext operationContext = null, CancellationToken cancellationToken = default(CancellationToken));
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
        /// <param name="operationContext">The operationContext for finding the entity (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A task promising the referenced entity.
        /// </returns>
        Task<T> GetAsync(IFindContext<T> operationContext = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}