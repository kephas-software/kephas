// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphEdge.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the graph edge class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Graphs
{
    using System.Diagnostics.Contracts;

    using Kephas.Dynamic;

    /// <summary>
    /// Defines a graph edge.
    /// </summary>
    public class GraphEdge : Expando, IGraphEdge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphEdge"/> class.
        /// </summary>
        /// <param name="from">The node from which the edge starts.</param>
        /// <param name="to">The node where the edge ends.</param>
        protected internal GraphEdge(GraphNode from, GraphNode to)
        {
            Contract.Requires(from != null);
            Contract.Requires(to != null);

            this.From = from;
            this.To = to;

            from.AddOutgoingEdge(this);
            to.AddIncomingEdge(this);
        }

        /// <summary>
        /// Gets the node from which the edge starts.
        /// </summary>
        /// <value>
        /// The node from which the edge starts.
        /// </value>
        public IGraphNode From { get; }

        /// <summary>
        /// Gets the node where the edge ends.
        /// </summary>
        /// <value>
        /// The node where the edge ends.
        /// </value>
        public IGraphNode To { get; }
    }
}