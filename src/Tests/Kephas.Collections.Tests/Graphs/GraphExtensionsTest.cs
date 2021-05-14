// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Graphs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Graphs;

    using NUnit.Framework;

    [TestFixture]
    public class GraphExtensionsTest
    {
        [Test]
        public void Merge_simple()
        {
            var graph = this.CreateGraph(new[]
                                        {
                                            Tuple.Create(2, 1),
                                            Tuple.Create(3, 4)
                                        });

            var sourceGraph = this.CreateGraph(new[]
                                        {
                                            Tuple.Create(5, 6),
                                            Tuple.Create(5, 7)
                                        });

            var merged = graph.Merge(sourceGraph);

            Assert.AreSame(merged, graph);
            Assert.AreEqual(7, merged.Nodes.Count);
            Assert.AreEqual(4, merged.Edges.Count);
        }

        [Test]
        public void GetConnectedSubgraphs_cyclic_simple()
        {
            var orderedGraph = this.CreateGraph<TestGraph, int>(new[]
                                        {
                                            Tuple.Create(2, 1),
                                            Tuple.Create(1, 2)
                                        });
            var subgraphs = orderedGraph.GetConnectedSubgraphs().ToList();
            Assert.AreEqual(1, subgraphs.Count);

            Assert.AreEqual(2, subgraphs[0].Nodes.Count);
            Assert.AreEqual(2, subgraphs[0].Edges.Count);
            Assert.AreSame(subgraphs[0].Nodes[0], orderedGraph.Nodes[0]);
            Assert.AreSame(subgraphs[0].Nodes[1], orderedGraph.Nodes[1]);
            Assert.AreSame(subgraphs[0].Edges[0], orderedGraph.Edges[0]);
            Assert.AreSame(subgraphs[0].Edges[1], orderedGraph.Edges[1]);
        }

        [Test]
        public void GetConnectedSubgraphs_two_not_connected_edges()
        {
            var orderedGraph = this.CreateGraph<TestGraph, int>(new[]
                                        {
                                            Tuple.Create(2, 1),
                                            Tuple.Create(4, 3)
                                        });
            var subgraphs = orderedGraph.GetConnectedSubgraphs().ToList();
            Assert.AreEqual(2, subgraphs.Count);

            Assert.AreEqual(2, subgraphs[0].Nodes.Count);
            Assert.AreEqual(1, subgraphs[0].Edges.Count);
            Assert.AreSame(subgraphs[0].Nodes[0], orderedGraph.Nodes[0]);
            Assert.AreSame(subgraphs[0].Nodes[1], orderedGraph.Nodes[1]);
            Assert.AreSame(subgraphs[0].Edges[0], orderedGraph.Edges[0]);

            Assert.AreEqual(2, subgraphs[1].Nodes.Count);
            Assert.AreEqual(1, subgraphs[1].Edges.Count);
            Assert.AreSame(subgraphs[1].Nodes[0], orderedGraph.Nodes[2]);
            Assert.AreSame(subgraphs[1].Nodes[1], orderedGraph.Nodes[3]);
            Assert.AreSame(subgraphs[1].Edges[0], orderedGraph.Edges[1]);
        }

        [Test]
        public void GetConnectedSubgraphs_three_connected_edges()
        {
            var orderedGraph = this.CreateGraph<TestGraph, int>(new[]
                                        {
                                            Tuple.Create(2, 1),
                                            Tuple.Create(4, 3),
                                            Tuple.Create(2, 3),
                                        });
            var subgraphs = orderedGraph.GetConnectedSubgraphs().ToList();
            Assert.AreEqual(1, subgraphs.Count);

            Assert.AreEqual(4, subgraphs[0].Nodes.Count);
            Assert.AreEqual(3, subgraphs[0].Edges.Count);
            Assert.AreSame(subgraphs[0].Nodes[0], orderedGraph.Nodes[0]);
            Assert.AreSame(subgraphs[0].Nodes[1], orderedGraph.Nodes[1]);
            Assert.AreSame(subgraphs[0].Nodes[2], orderedGraph.Nodes[3]);
            Assert.AreSame(subgraphs[0].Nodes[3], orderedGraph.Nodes[2]);
            Assert.AreSame(subgraphs[0].Edges[0], orderedGraph.Edges[0]);
            Assert.AreSame(subgraphs[0].Edges[1], orderedGraph.Edges[2]);
            Assert.AreSame(subgraphs[0].Edges[2], orderedGraph.Edges[1]);
        }

        private Graph<TItem, object> CreateGraph<TItem>(IEnumerable<Tuple<TItem, TItem>> edges)
        {
            return this.CreateGraph<Graph<TItem, object>, TItem>(edges);
        }

        private T CreateGraph<T, TItem>(IEnumerable<Tuple<TItem, TItem>> edges) where T : Graph, new()
        {
            var nodes = new HashSet<TItem>();
            edges.ForEach(e => nodes.AddRange(new[] { e.Item1, e.Item2 }));
            var graph = new T();
            var nodesDict = nodes.ToDictionary(
                n => n,
                n =>
                    {
                        var gnode = graph.AddNode();
                        ((dynamic)gnode).Value = n;
                        return gnode;
                    });

            edges.ForEach(e => graph.AddEdge(nodesDict[e.Item1], nodesDict[e.Item2]));

            return graph;
        }

        private class TestNode : GraphNode
        {
            public override string ToString()
            {
                return (this as dynamic).Value.ToString();
            }
        }

        private class TestEdge : GraphEdge
        {
            protected internal TestEdge(GraphNode @from, GraphNode to)
                : base(@from, to)
            {
            }

            public override string ToString()
            {
                return $"{this.From} > {this.To}";
            }
        }

        private class TestGraph : Graph
        {
            protected internal override Graph CreateSubgraph()
            {
                return new TestGraph();
            }

            protected override GraphNode CreateNode()
            {
                return new TestNode();
            }

            protected override GraphEdge CreateEdge(GraphNode @from, GraphNode to)
            {
                return new TestEdge(@from, to);
            }
        }
    }
}