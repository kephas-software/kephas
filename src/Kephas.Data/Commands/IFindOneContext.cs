﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFindOneContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    public interface IFindOneContext : IFindContextBase
    {
        /// <summary>
        /// Gets the criteria of the entity to find.
        /// </summary>
        /// <value>
        /// The criteria of the entity to find.
        /// </value>
        Expression Criteria { get; }
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