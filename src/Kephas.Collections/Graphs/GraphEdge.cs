// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphEdge.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the graph edge class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Graphs
{
    using Kephas.Diagnostics.Contracts;
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
            Requires.NotNull(from, nameof(from));
            Requires.NotNull(to, nameof(to));

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
        public virtual IGraphNode From { get; }

        /// <summary>
        /// Gets the node where the edge ends.
        /// </summary>
        /// <value>
        /// The node where the edge ends.
        /// </value>
        public virtual IGraphNode To { get; }

        /// <summary>
        /// Returns a string that represents the edge.
        /// </summary>
        /// <returns>
        /// A string that represents the edge.
        /// </returns>
        public override string ToString()
        {
            return $"{this.From} -> {this.To}";
        }
    }

    /// <summary>
    /// Defines a graph edge connecting nodes holding values.
    /// </summary>
    /// <typeparam name="TNodeValue">Type of the node value.</typeparam>
    /// <typeparam name="TEdgeValue">Type of the edge value.</typeparam>
    public class GraphEdge<TNodeValue, TEdgeValue> : GraphEdge, IGraphEdge<TNodeValue, TEdgeValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphEdge{TNodeValue, TEdgeValue}"/> class.
        /// </summary>
        /// <param name="from">The node from which the edge starts.</param>
        /// <param name="to">The node where the edge ends.</param>
        public GraphEdge(GraphNode<TNodeValue> from, GraphNode<TNodeValue> to)
            : base(from, to)
        {
        }

        /// <summary>
        /// Gets the node from which the edge starts.
        /// </summary>
        /// <value>
        /// The node from which the edge starts.
        /// </value>
        public new IGraphNode<TNodeValue> From => (IGraphNode<TNodeValue>)base.From;

        /// <summary>
        /// Gets the node where the edge ends.
        /// </summary>
        /// <value>
        /// The node where the edge ends.
        /// </value>
        public new IGraphNode<TNodeValue> To => (IGraphNode<TNodeValue>)base.To;

        /// <summary>
        /// Gets or sets the value of the edge.
        /// </summary>
        public TEdgeValue? Value { get; set; }
    }
}