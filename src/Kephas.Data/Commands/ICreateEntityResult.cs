// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICreateEntityResult.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the ICreateEntityResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    /// <summary>
    /// Contract for the create entity result.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public interface ICreateEntityResult<out TEntity> : IDataCommandResult
    {
        /// <summary>
        /// Gets the found entity or <c>null</c> if no entity could be found.
        /// </summary>
        /// <value>
        /// The found entity.
        /// </value>
        TEntity Entity { get; }
    }
}