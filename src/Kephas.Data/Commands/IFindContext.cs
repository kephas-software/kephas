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