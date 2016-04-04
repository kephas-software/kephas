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

    /// <summary>
    /// An unoriented graph edge.
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
            Contract.Requires(from != null);
            Contract.Requires(to != null);

            from.AddIncomingEdge(this);
            to.AddOutgoingEdge(this);
        }
    }
}