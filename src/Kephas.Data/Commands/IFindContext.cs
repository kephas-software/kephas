// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFindContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for find contexts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    /// <summary>
    /// Contract for find contexts.
    /// </summary>
    public interface IFindContext : IFindContextBase
    {
        /// <summary>
        /// Gets the identifier of the entity to find.
        /// </summary>
        /// <value>
        /// The identifier of the entity.
        /// </value>
        object Id { get; }
    }

    /// <summary>
    /// Generic contract for find contexts.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public interface IFindContext<TEntity> : IFindContext
    {
    }
}