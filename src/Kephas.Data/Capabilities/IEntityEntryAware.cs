// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityEntryAware.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEntityEntryAware interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Capabilities
{
    /// <summary>
    /// Annotates the entities which are aware about their entity entry.
    /// </summary>
    public interface IEntityEntryAware
    {
        /// <summary>
        /// Gets the entity entry.
        /// </summary>
        /// <returns>
        /// The entity entry.
        /// </returns>
        IEntityEntry GetEntityEntry();

        /// <summary>
        /// Sets the entity entry.
        /// </summary>
        /// <param name="entityEntry">The entity entry.</param>
        void SetEntityEntry(IEntityEntry entityEntry);
    }
}