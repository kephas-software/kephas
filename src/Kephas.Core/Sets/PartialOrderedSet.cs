// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartialOrderedSet.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines the PartialOrderedSet type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Sets
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Graphs;
    using Kephas.Resources;

    /// <summary>
    /// A partial ordered set.
    /// </summary>
    /// <typeparam name="TValue">The type of the set values.</typeparam>
    public class PartialOrderedSet<TValue> : IEnumerable<TValue>
    {
        /// <summary>
        /// The order graph.
        /// </summary>
        private readonly Graph<TValue> orderGraph = new Graph<TValue>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialOrderedSet{TValue}"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="partialComparer">The partial comparer function.</param>
        public PartialOrderedSet(IEnumerable<TValue> values, Func<TValue, TValue, int?> partialComparer)
        {
            Requires.NotNull(values, nameof(values));
            Requires.NotNull(partialComparer, nameof(partialComparer));

            this.InitializeOrderGraph(values.ToList(), partialComparer);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through
        /// the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<TValue> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes the order graph.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="partialComparer">The partial comparer function.</param>
        private void InitializeOrderGraph(IList<TValue> values, Func<TValue, TValue, int?> partialComparer)
        {
            foreach (var value in values)
            {
                this.orderGraph.AddNode(value);
            }

            this.InitializeOrderGraphEdges(values, partialComparer);

            var orderedValues = new List<TValue>();
            var subgraphs = this.orderGraph.GetConnectedSubgraphs();
            foreach (var subgraph in subgraphs)
            {
                this.AddMissingOrderEdges(subgraph);
                orderedValues.AddRange(this.GetOrderedValues(subgraph));
            }
        }

        /// <summary>
        /// Gets the ordered values from the nodes of the connected graph.
        /// </summary>
        /// <param name="connectedGraph">The connected graph.</param>
        /// <returns>
        /// An enumeration of values, ordered.
        /// </returns>
        private IEnumerable<TValue> GetOrderedValues(Graph<TValue> connectedGraph)
        {
            var orderedNodes = new List<IGraphNode<TValue>>();
            var eligibleNodes = new List<IGraphNode<TValue>>(connectedGraph.Nodes.OfType<IGraphNode<TValue>>());

            while (eligibleNodes.Count > 0)
            {
                var minNodes = eligibleNodes.Where(n => n.OutgoingEdges.All(e => orderedNodes.Contains(e.To))).ToList();
                if (minNodes.Count == 0)
                {
                    // it means that there are cycles in the graph, which indicate
                    // an unsound graph
                    throw new InvalidOperationException(Strings.PartialOrderedSet_BadComparer_ProducesCycles_Exception);
                }

                orderedNodes.AddRange(minNodes);
                eligibleNodes.RemoveAll(n => minNodes.Contains(n));
            }

            return orderedNodes.Select(n => n.Value);
        }

        /// <summary>
        /// Initializes the order graph edges.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="partialComparer">The partial comparer function.</param>
        private void InitializeOrderGraphEdges(IList<TValue> values, Func<TValue, TValue, int?> partialComparer)
        {
            foreach (IGraphNode<TValue> node in this.orderGraph.Nodes)
            {
                foreach (var value in values)
                {
                    if (object.Equals(node.Value, value))
                    {
                        continue;
                    }

                    var comparisonResult = partialComparer(node.Value, value);
                    if (!comparisonResult.HasValue || comparisonResult == 0)
                    {
                        continue;
                    }

                    var otherNode = this.orderGraph.FindNodesByValue(value).Single();

                    if (comparisonResult < 0)
                    {
                        // node < otherNode
                        this.orderGraph.AddEdge(node, otherNode);
                    }
                    else
                    {
                        // otherNode < node
                        this.orderGraph.AddEdge(otherNode, node);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the missing order edges.
        /// </summary>
        /// <param name="subgraph">The subgraph.</param>
        private void AddMissingOrderEdges(Graph<TValue> subgraph)
        {
            var edgesToProcess = new Queue<IGraphEdge<TValue>>(subgraph.Edges.OfType<IGraphEdge<TValue>>());

            while (edgesToProcess.Count > 0)
            {
                var edge = edgesToProcess.Dequeue();
                var from = edge.From;
                var toOutgoing = edge.To.OutgoingEdges.Select(e => e.To);
                foreach (var to in toOutgoing)
                {
                    if (!subgraph.HasEdge(from, to))
                    {
                        var newEdge = (IGraphEdge<TValue>)subgraph.AddEdge(from, to);
                        edgesToProcess.Enqueue(newEdge);
                    }
                }
            }
        }
    }
}