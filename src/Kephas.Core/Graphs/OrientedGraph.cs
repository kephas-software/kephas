// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrientedGraph.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the oriented graph class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Graphs
{
    /// <summary>
    /// Defines an oriented graph.
    /// </summary>
    public class OrientedGraph : GraphBase
    {
        /// <summary>
        /// Creates a subgraph.
        /// </summary>
        /// <returns>
        /// The new subgraph.
        /// </returns>
        public override GraphBase CreateSubgraph()
        {
            return new OrientedGraph();
        }
    }
}