// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAggregatable.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IAggregatable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
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
        IAggregatable GetGraphRoot();

        /// <summary>
        /// Gets the flattened graph asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// The flattened graph asynchronous.
        /// </returns>
        Task<IEnumerable<object>> GetFlattenedGraphAsync(
          IGraphOperationContext operationContext,
          CancellationToken cancellationToken = default(CancellationToken));
    }
}