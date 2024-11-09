// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAggregatable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAggregatable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Capabilities
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Contract for an entity's ability of being aggregated within a graph.
    /// </summary>
    public interface IAggregatable
    {
        /// <summary>
        /// Gets the root of the entity graph.
        /// </summary>
        /// <returns>
        /// The graph root.
        /// </returns>
        IAggregatable? GetGraphRoot();

        /// <summary>
        /// Gets the flattened structural entity graph excluding the loose parts (only the internal structure).
        /// </summary>
        /// <returns>
        /// The flattened structural entity graph.
        /// </returns>
        IEnumerable<object> GetStructuralEntityGraph();

        /// <summary>
        /// Gets the flattened entity graph asynchronously.
        /// This may include also loose parts which are asynchronously loaded.
        /// If no loose parts must be loaded, then the result is the same with <see cref="GetStructuralEntityGraph"/>.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the flattened entity graph.
        /// </returns>
        Task<IEnumerable<object>> GetFlattenedEntityGraphAsync(
          IGraphOperationContext operationContext,
          CancellationToken cancellationToken = default);
    }
}