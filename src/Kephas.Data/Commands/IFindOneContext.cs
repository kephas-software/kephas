// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFindOneContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IFindOneContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Interface for data operation contexts of the <see cref="IFindOneCommand"/>.
    /// </summary>
    public interface IFindOneContext : IDataOperationContext
    {
        /// <summary>
        /// Gets the criteria of the entity to find.
        /// </summary>
        /// <value>
        /// The criteria of the entity to find.
        /// </value>
        Expression Criteria { get; }

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
    /// Generic interface for data operation contexts of the <see cref="IFindOneCommand"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public interface IFindOneContext<TEntity> : IFindOneContext
    {
        /// <summary>
        /// Gets the criteria of the entity to find.
        /// </summary>
        /// <remarks>
        /// Overrides the untyped expression from the base interface
        /// to provide LINQ-support.
        /// </remarks>
        /// <value>
        /// The criteria of the entity to find.
        /// </value>
        new Expression<Func<TEntity, bool>> Criteria { get; }
    }
}