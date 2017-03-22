// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFindResult.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IFindResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    /// <summary>
    /// Contract for the find result.
    /// </summary>
    public interface IFindResult : IDataCommandResult
    {
        /// <summary>
        /// Gets the found entity or <c>null</c> if no entity could be found.
        /// </summary>
        /// <value>
        /// The found entity.
        /// </value>
        object Entity { get; }
    }
}