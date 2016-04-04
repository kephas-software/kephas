// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnorientedGraph.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    public class UnorientedGraph : GraphBase
    {
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

        /// <summary>
        /// Creates a subgraph.
        /// </summary>
        /// <returns>
        /// The new subgraph.
        /// </returns>
        public override GraphBase CreateSubgraph()
        {
            return new UnorientedGraph();
        }
    }
}