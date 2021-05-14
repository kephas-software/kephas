// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnorientedGraphEdge.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the unoriented graph edge class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Graphs
{
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Defines an unoriented graph edge.
    /// </summary>
    public class UnorientedGraphEdge : GraphEdge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnorientedGraphEdge"/> class.
        /// </summary>
        /// <param name="from">The node from which the edge starts.</param>
        /// <param name="to">The node where the edge ends.</param>
        public UnorientedGraphEdge(GraphNode from, GraphNode to)
            : base(from, to)
        {
            Requires.NotNull(from, nameof(from));
            Requires.NotNull(to, nameof(to));

            from.AddIncomingEdge(this);
            to.AddOutgoingEdge(this);
        }
    }

    /// <summary>
    /// Defines an unoriented graph edge connecting nodes holding values.
    /// </summary>
    /// <typeparam name="TNodeValue">Type of the node value.</typeparam>
    /// <typeparam name="TEdgeValue">Type of the edge value.</typeparam>
    public class UnorientedGraphEdge<TNodeValue, TEdgeValue> : GraphEdge<TNodeValue, TEdgeValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnorientedGraphEdge{TNodeValue, TEdgeValue}"/> class.
        /// </summary>
        /// <param name="from">The node from which the edge starts.</param>
        /// <param name="to">The node where the edge ends.</param>
        public UnorientedGraphEdge(GraphNode<TNodeValue> from, GraphNode<TNodeValue> to)
            : base(from, to)
        {
        }
    }
}