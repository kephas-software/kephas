// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFindContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for find contexts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;

    /// <summary>
    /// Contract for find contexts.
    /// </summary>
    public interface IFindContext : IDataOperationContext
    {
        /// <summary>
        /// Gets the identifier of the entity to find.
        /// </summary>
        /// <value>
        /// The identifier of the entity.
        /// </value>
        Id Id { get; }

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        Type EntityType { get; }

        /// <summary>
        /// Gets a value indicating whether to throw an exception if an entity is not found.
        /// </summary>
        /// <value>
        /// <c>true</c>true to throw an exception if an entity is not found, otherwise <c>false</c>.
        /// </value>
        bool ThrowIfNotFound { get; }
    }

    /// <summary>
    /// Generic contract for find contexts.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public interface IFindContext<TEntity> : IFindContext
    {
    }
}