namespace Kephas.Core.Tests.Graphs
{
    using System.Linq;

    using Kephas.Graphs;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class GraphNodeTest
    {
        [Test]
        public void AddIncomingEdge_success()
        {
            var node = new GraphNode();
            var fromNode = new GraphNode();

            // AddIncomingEdge called implicitely
            var edge = new GraphEdge(fromNode, node);

            Assert.AreEqual(1, node.IncomingEdges.Count);
            Assert.AreEqual(edge, node.IncomingEdges.First());
            Assert.AreEqual(0, node.OutgoingEdges.Count);
            Assert.AreEqual(1, node.ConnectedEdges.Count);
            Assert.AreEqual(edge, node.ConnectedEdges.First());

            Assert.AreEqual(1, node.ConnectedNodes.Count);
            Assert.AreEqual(fromNode, node.ConnectedNodes.First());
        }

        [Test]
        public void AddOutgoingEdge_success()
        {
            var node = new GraphNode();
            var toNode = new GraphNode();

            // AddOutgoingEdge called implicitely
            var edge = new GraphEdge(node, toNode);

            Assert.AreEqual(0, node.IncomingEdges.Count);
            Assert.AreEqual(1, node.OutgoingEdges.Count);
            Assert.AreEqual(edge, node.OutgoingEdges.First());
            Assert.AreEqual(1, node.ConnectedEdges.Count);
            Assert.AreEqual(edge, node.ConnectedEdges.First());

            Assert.AreEqual(1, node.ConnectedNodes.Count);
            Assert.AreEqual(toNode, node.ConnectedNodes.First());
        }
    }
}