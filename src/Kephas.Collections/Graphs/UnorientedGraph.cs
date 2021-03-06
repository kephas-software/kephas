﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnorientedGraph.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the unoriented graph class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Graphs
{
    /// <summary>
    /// Defines an unoriented graph.
    /// </summary>
    public class UnorientedGraph : Graph
    {
        /// <summary>
        /// Creates a subgraph.
        /// </summary>
        /// <returns>
        /// The new subgraph.
        /// </returns>
        protected internal override Graph CreateSubgraph()
        {
            return new UnorientedGraph();
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
            return new UnorientedGraphEdge(from, to);
        }
    }

    /// <summary>
    /// Defines an unoriented graph with value nodes.
    /// </summary>
    /// <typeparam name="TNodeValue">Type of the node value.</typeparam>
    /// <typeparam name="TEdgeValue">Type of the edge values.</typeparam>
    public class UnorientedGraph<TNodeValue, TEdgeValue> : Graph<TNodeValue, TEdgeValue>
    {
        /// <summary>
        /// Creates a subgraph.
        /// </summary>
        /// <returns>
        /// The new subgraph.
        /// </returns>
        protected internal override Graph CreateSubgraph()
        {
            return new UnorientedGraph<TNodeValue, TEdgeValue>();
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
            return new UnorientedGraphEdge<TNodeValue, TEdgeValue>((GraphNode<TNodeValue>)from, (GraphNode<TNodeValue>)to);
        }
    }
}