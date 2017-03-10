// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnorientedGraphEdge.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the unoriented graph edge class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Graphs
{
    using System.Diagnostics.Contracts;

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
    public class UnorientedGraphEdge<TNodeValue> : GraphEdge<TNodeValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnorientedGraphEdge{TNodeValue}"/> class.
        /// </summary>
        /// <param name="from">The node from which the edge starts.</param>
        /// <param name="to">The node where the edge ends.</param>
        public UnorientedGraphEdge(GraphNode<TNodeValue> from, GraphNode<TNodeValue> to)
            : base(from, to)
        {
        }
    }
}