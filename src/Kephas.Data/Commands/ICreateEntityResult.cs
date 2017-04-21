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
    using Kephas.Data.Capabilities;

    /// <summary>
    /// Contract for the create entity result.
    /// </summary>
    public interface ICreateEntityResult : IDataCommandResult
    {
        /// <summary>
        /// Gets the newly created entity.
        /// </summary>
        /// <value>
        /// The new entity.
        /// </value>
        object Entity { get; }

        /// <summary>
        /// Gets information describing the newly created entity.
        /// </summary>
        /// <value>
        /// Information describing the new entity.
        /// </value>
        IEntityInfo EntityInfo { get; }
    }
}