// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the graph extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Graphs
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Kephas.Collections;

    /// <summary>
    /// Extension methods for graphs.
    /// </summary>
    public static class GraphExtensions
    {
        /// <summary>
        /// Gets the connected subgraphs.
        /// </summary>
        /// <typeparam name="TGraph">Type of the graph.</typeparam>
        /// <param name="graph">The graph to act on.</param>
        /// <returns>
        /// The connected subgraphs.
        /// </returns>
        public static ICollection<TGraph> GetConnectedSubgraphs<TGraph>(this TGraph graph)
            where TGraph : GraphBase
        {
            Contract.Requires(graph != null);

            var subgraphs = new List<TGraph>();

            var nodes = new List<IGraphNode>(graph.Nodes);
            while (nodes.Count > 0)
            {
                var subgraph = (TGraph)graph.CreateSubgraph();
                var connectedNodes = new List<IGraphNode> { nodes[0] };

                while (connectedNodes.Count > 0)
                {
                    var nextNodes = new List<IGraphNode>();
                    connectedNodes.ForEach(
                        n =>
                            {
                                if (subgraph.AddNode(n))
                                {
                                    nextNodes.AddRange(n.ConnectedNodes);
                                    nodes.Remove(n);
                                }
                            });
                    connectedNodes.ForEach(n => n.ConnectedEdges.ForEach(e => subgraph.AddEdge(e)));

                    connectedNodes = nextNodes;
                }

                subgraphs.Add(subgraph);
            }

            return subgraphs;
        }
    }
}