// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphNode.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the graph node class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Graphs
{
    using System.Collections.Generic;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// Defines a graph node.
    /// </summary>
    public class GraphNode : Expando, IGraphNode
    {
        /// <summary>
        /// The incoming edges.
        /// </summary>
        private readonly List<IGraphEdge> incomingEdges;

        /// <summary>
        /// The outgoing edges.
        /// </summary>
        private readonly List<IGraphEdge> outgoingEdges;

        /// <summary>
        /// The connected nodes.
        /// </summary>
        private readonly List<IGraphNode> connectedNodes;

        /// <summary>
        /// The connected edges.
        /// </summary>
        private readonly List<IGraphEdge> connectedEdges;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphNode"/> class.
        /// </summary>
        public GraphNode()
        {
            this.incomingEdges = new List<IGraphEdge>();
            this.outgoingEdges = new List<IGraphEdge>();
            this.connectedNodes = new List<IGraphNode>();
            this.connectedEdges = new List<IGraphEdge>();
        }

        /// <summary>
        /// Gets the incoming nodes from this node.
        /// </summary>
        /// <value>
        /// The incoming edges.
        /// </value>
        public IReadOnlyCollection<IGraphEdge> IncomingEdges => this.incomingEdges;

        /// <summary>
        /// Gets the outgoing edges from this node.
        /// </summary>
        /// <value>
        /// The outgoing edges.
        /// </value>
        public IReadOnlyCollection<IGraphEdge> OutgoingEdges => this.outgoingEdges;

        /// <summary>
        /// Gets the connected edges.
        /// </summary>
        /// <value>
        /// The connected edges.
        /// </value>
        public IReadOnlyCollection<IGraphEdge> ConnectedEdges => this.connectedEdges;

        /// <summary>
        /// Gets the connected nodes.
        /// </summary>
        /// <value>
        /// The connected nodes.
        /// </value>
        public IReadOnlyCollection<IGraphNode> ConnectedNodes => this.connectedNodes;

        /// <summary>
        /// Adds an incoming edge.
        /// </summary>
        /// <param name="edge">The edge.</param>
        protected internal void AddIncomingEdge(GraphEdge edge)
        {
            Requires.NotNull(edge, nameof(edge));

            this.incomingEdges.Add(edge);
            this.AddConnectedNode(edge.From);
            this.AddConnectedEdge(edge);
        }

        /// <summary>
        /// Adds an outgoing edge.
        /// </summary>
        /// <param name="edge">The edge.</param>
        protected internal void AddOutgoingEdge(GraphEdge edge)
        {
            Requires.NotNull(edge, nameof(edge));

            this.outgoingEdges.Add(edge);
            this.AddConnectedNode(edge.To);
            this.AddConnectedEdge(edge);
        }

        /// <summary>
        /// Adds a connected node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// <c>true</c> if the node was added, <c>false</c> if it was already contained.
        /// </returns>
        protected virtual bool AddConnectedNode(IGraphNode node)
        {
            if (this.connectedNodes.Contains(node))
            {
                return false;
            }

            this.connectedNodes.Add(node);
            return true;
        }

        /// <summary>
        /// Adds a connected edge.
        /// </summary>
        /// <param name="edge">The edge.</param>
        /// <returns>
        /// <c>true</c> if the edge was added, <c>false</c> if it was already contained.
        /// </returns>
        protected virtual bool AddConnectedEdge(IGraphEdge edge)
        {
            if (this.connectedEdges.Contains(edge))
            {
                return false;
            }

            this.connectedEdges.Add(edge);
            return true;
        }
    }

    /// <summary>
    /// Defines a graph node holding a value.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public class GraphNode<TValue> : GraphNode, IGraphNode<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphNode{TValue}"/> class.
        /// </summary>
        public GraphNode()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphNode{TValue}"/> class.
        /// </summary>
        /// <param name="value">The node value.</param>
        public GraphNode(TValue value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the node value.
        /// </summary>
        /// <value>
        /// The node value.
        /// </value>
        public TValue Value { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{{{this.Value}}}";
        }
    }
}