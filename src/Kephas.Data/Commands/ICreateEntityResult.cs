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
    public interface ICreateEntityResult : IDataCommandResult
    {
        /// <summary>
        /// Gets or sets the found entity or <c>null</c> if no entity could be found.
        /// </summary>
        /// <value>
        /// The found entity.
        /// </value>
        object Entity { get; set; }
    }
}