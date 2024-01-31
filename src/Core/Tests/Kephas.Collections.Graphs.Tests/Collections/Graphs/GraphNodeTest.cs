// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphNodeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Collections.Graphs;
using Xunit;

namespace Kephas.Tests.Collections.Graphs
{
        public class GraphNodeTest
    {
        [Fact]
        public void AddIncomingEdge_success()
        {
            var node = new GraphNode();
            var fromNode = new GraphNode();

            // AddIncomingEdge called implicitely
            var edge = new GraphEdge(fromNode, node);

            Assert.Equal(1, node.IncomingEdges.Count);
            Assert.Equal(edge, node.IncomingEdges.First());
            Assert.Equal(0, node.OutgoingEdges.Count);
            Assert.Equal(1, node.ConnectedEdges.Count);
            Assert.Equal(edge, node.ConnectedEdges.First());

            Assert.Equal(1, node.ConnectedNodes.Count);
            Assert.Equal(fromNode, node.ConnectedNodes.First());
        }

        [Fact]
        public void AddOutgoingEdge_success()
        {
            var node = new GraphNode();
            var toNode = new GraphNode();

            // AddOutgoingEdge called implicitely
            var edge = new GraphEdge(node, toNode);

            Assert.Equal(0, node.IncomingEdges.Count);
            Assert.Equal(1, node.OutgoingEdges.Count);
            Assert.Equal(edge, node.OutgoingEdges.First());
            Assert.Equal(1, node.ConnectedEdges.Count);
            Assert.Equal(edge, node.ConnectedEdges.First());

            Assert.Equal(1, node.ConnectedNodes.Count);
            Assert.Equal(toNode, node.ConnectedNodes.First());
        }
    }
}
