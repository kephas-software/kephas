// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDeleteEntityContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDeleteEntityContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for delete entity context.
    /// </summary>
    public interface IDeleteEntityContext : IDataOperationContext
    {
        /// <summary>
        /// Gets the entities to delete.
        /// </summary>
        /// <value>
        /// The entities to delete.
        /// </value>
        IEnumerable<object> Entities { get; }
    }
}