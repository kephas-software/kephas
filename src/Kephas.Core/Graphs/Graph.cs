// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Graph.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the graph base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Graphs
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Resources;

    /// <summary>
    /// Defines an oriented graph.
    /// </summary>
    public class Graph
    {
        /// <summary>
        /// The nodes.
        /// </summary>
        private readonly List<IGraphNode> nodes = new List<IGraphNode>();

        /// <summary>
        /// The edges.
        /// </summary>
        private readonly List<IGraphEdge> edges = new List<IGraphEdge>();

        /// <summary>
        /// Gets a list of the graph nodes.
        /// </summary>
        public IReadOnlyList<IGraphNode> Nodes => this.nodes;

        /// <summary>
        /// Gets a list of the graph edges.
        /// </summary>
        public IReadOnlyList<IGraphEdge> Edges => this.edges;

        /// <summary>
        /// Adds a node to the graph and returns it.
        /// </summary>
        /// <returns>
        /// The new node.
        /// </returns>
        public virtual IGraphNode AddNode()
        {
            var node = this.CreateNode();
            this.nodes.Add(node);
            return node;
        }

        /// <summary>
        /// Adds an edge between the two nodes.
        /// </summary>
        /// <param name="from">The starting node.</param>
        /// <param name="to">The ending node.</param>
        /// <returns>
        /// The new edge.
        /// </returns>
        public virtual IGraphEdge AddEdge(IGraphNode from, IGraphNode to)
        {
            var edge = this.CreateEdge((GraphNode)from, (GraphNode)to);
            this.edges.Add(edge);
            return edge;
        }

        /// <summary>
        /// Creates a subgraph.
        /// </summary>
        /// <returns>
        /// The new subgraph.
        /// </returns>
        protected internal virtual Graph CreateSubgraph()
        {
            return new Graph();
        }

        /// <summary>
        /// Adds a node to the graph, if not already contained.
        /// </summary>
        /// <param name="node">The node to be added.</param>
        /// <returns>
        /// <c>true</c> if the node was added, <c>false</c> if it was already contained.
        /// </returns>
        protected internal bool AddNode(IGraphNode node)
        {
            Contract.Requires(node != null);

            if (this.nodes.Contains(node))
            {
                return false;
            }

            this.nodes.Add(node);
            return true;
        }

        /// <summary>
        /// Adds an edge to the graph, if not already contained.
        /// </summary>
        /// <param name="edge">The edge to be added.</param>
        /// <returns>
        /// <c>true</c> if the edge was added, <c>false</c> if it was already contained.
        /// </returns>
        protected internal bool AddEdge(IGraphEdge edge)
        {
            Requires.NotNull(edge, nameof(edge));

            if (this.edges.Contains(edge))
            {
                return false;
            }

            this.edges.Add(edge);
            return true;
        }

        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <returns>
        /// The new node.
        /// </returns>
        protected virtual GraphNode CreateNode()
        {
            return new GraphNode();
        }

        /// <summary>
        /// Creates an edge between two nodes.
        /// </summary>
        /// <param name="from">The starting node.</param>
        /// <param name="to">The ending node.</param>
        /// <returns>
        /// The new edge.
        /// </returns>
        protected virtual GraphEdge CreateEdge(GraphNode from, GraphNode to)
        {
            return new GraphEdge(from, to);
        }
    }

    /// <summary>
    /// Defines an oriented graph with value nodes.
    /// </summary>
    /// <typeparam name="TNodeValue">Type of the node value.</typeparam>
    public class Graph<TNodeValue> : Graph
    {
        /// <summary>
        /// Adds a node to the graph and returns it.
        /// </summary>
        /// <returns>
        /// The new node.
        /// </returns>
        public new IGraphNode<TNodeValue> AddNode()
        {
            return (IGraphNode<TNodeValue>)base.AddNode();
        }

        /// <summary>
        /// Adds a node with the given value to the graph and returns it.
        /// </summary>
        /// <param name="value">The node value.</param>
        /// <returns>
        /// The new node.
        /// </returns>
        public IGraphNode<TNodeValue> AddNode(TNodeValue value)
        {
            var node = (IGraphNode<TNodeValue>)base.AddNode();
            node.Value = value;
            return node;
        }

        /// <summary>
        /// Adds an edge between the two nodes.
        /// </summary>
        /// <param name="from">The starting node.</param>
        /// <param name="to">The ending node.</param>
        /// <returns>
        /// The new edge.
        /// </returns>
        public IGraphEdge<TNodeValue> AddEdge(IGraphNode<TNodeValue> from, IGraphNode<TNodeValue> to)
        {
            return (IGraphEdge<TNodeValue>)base.AddEdge(from, to);
        }

        /// <summary>
        /// Adds an edge between two nodes having the given values.
        /// If for a value a corresponding node cannot be found, it will be created.
        /// If multiple nodes with the same value are found, an exception occurs.
        /// </summary>
        /// <param name="fromValue">The starting node identified by this value.</param>
        /// <param name="toValue">The ending node identified by this value.</param>
        /// <returns>
        /// The new edge.
        /// </returns>
        public IGraphEdge<TNodeValue> AddEdge(TNodeValue fromValue, TNodeValue toValue)
        {
            var fromNodes = this.FindNodesByValue(fromValue).Take(2).ToList();
            if (fromNodes.Count > 1)
            {
                throw new AmbiguousMatchException(string.Format(Strings.GraphBaseOfTNodeValue_AmbiguousMatchForValue_Exception, fromValue));
            }

            var from = fromNodes.Count == 0 ? this.AddNode(fromValue) : fromNodes[0];

            var toNodes = this.FindNodesByValue(toValue).Take(2).ToList();
            if (toNodes.Count > 1)
            {
                throw new AmbiguousMatchException(string.Format(Strings.GraphBaseOfTNodeValue_AmbiguousMatchForValue_Exception, toValue));
            }

            var to = toNodes.Count == 0 ? this.AddNode(toValue) : toNodes[0];

            return (IGraphEdge<TNodeValue>)base.AddEdge(from, to);
        }

        /// <summary>
        /// Finds the nodes holding the provided value.
        /// </summary>
        /// <param name="value">The node value.</param>
        /// <returns>
        /// An enumeration of found nodes.
        /// </returns>
        public IEnumerable<IGraphNode<TNodeValue>> FindNodesByValue(TNodeValue value)
        {
            return this.Nodes.OfType<IGraphNode<TNodeValue>>().Where(n => object.Equals(n.Value, value));
        }

        /// <summary>
        /// Creates a subgraph.
        /// </summary>
        /// <returns>
        /// The new subgraph.
        /// </returns>
        protected internal override Graph CreateSubgraph()
        {
            return new Graph<TNodeValue>();
        }

        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <returns>
        /// The new node.
        /// </returns>
        protected override GraphNode CreateNode()
        {
            return new GraphNode<TNodeValue>();
        }

        /// <summary>
        /// Creates an edge between two nodes.
        /// </summary>
        /// <param name="from">The starting node.</param>
        /// <param name="to">The ending node.</param>
        /// <returns>
        /// The new edge.
        /// </returns>
        protected override GraphEdge CreateEdge(GraphNode from, GraphNode to)
        {
            return new GraphEdge<TNodeValue>((GraphNode<TNodeValue>)from, (GraphNode<TNodeValue>)to);
        }
    }
}