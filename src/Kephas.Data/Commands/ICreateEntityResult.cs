// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICreateEntityResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
        IEntityEntry EntityEntry { get; }
    }
}