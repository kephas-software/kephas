// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueryFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IQueryFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Entities
{
    using System.Linq;

    /// <summary>
    /// Interface for linq meta data.
    /// </summary>
    public interface IQueryFactory
    {
        /// <summary>
        /// Returns the datasource to use in a Linq query for the entity type specified.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to get the datasource for.</typeparam>
        /// <returns>
        /// The requested datasource.
        /// </returns>
        IQueryable<TEntity> ToQueryable<TEntity>()
            where TEntity : class;
    }
}