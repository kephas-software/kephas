﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncAggregatable.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IAsyncAggregatable interface.
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
    public interface IAsyncAggregatable
    {
        /// <summary>
        /// Gets the root of the entity graph.
        /// </summary>
        /// <returns>
        /// The graph root.
        /// </returns>
        IAsyncAggregatable GetGraphRoot();

        /// <summary>
        /// Gets the flattened graph asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// The flattened graph asynchronous.
        /// </returns>
        Task<IEnumerable<object>> GetFlattenedGraphAsync(
          IGraphDataContext context,
          CancellationToken cancellationToken = default(CancellationToken));
    }
}