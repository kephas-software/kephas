// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphBase.cs" company="Quartz Software SRL">
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

    /// <summary>
    /// Base class for graphs.
    /// </summary>
    public abstract class GraphBase
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
        public abstract GraphBase CreateSubgraph();

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
            Contract.Requires(edge != null);

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
}