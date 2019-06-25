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
    /// Factory contract for creating entity queries.
    /// </summary>
    public interface IQueryFactory
    {
        /// <summary>
        /// Returns the data source to use in a Linq query for the entity type specified.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to get the data source for.</typeparam>
        /// <returns>
        /// The requested data source.
        /// </returns>
        IQueryable<TEntity> ToQueryable<TEntity>()
            where TEntity : class;
    }
}