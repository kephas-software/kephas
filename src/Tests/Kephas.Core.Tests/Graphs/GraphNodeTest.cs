namespace Kephas.Core.Tests.Graphs
{
    using System.Linq;

    using Kephas.Graphs;

    using NUnit.Framework;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    [TestFixture]
    public class GraphNodeTest
    {
        [Test]
        public void AddIncomingEdge_success()
        {
            var node = new GraphNode();
            var fromNode = new GraphNode();
            var edge = Mock.Create<GraphEdge>();
            edge.Arrange(e => e.From).Returns(fromNode);
            node.AddIncomingEdge(edge);

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
            var edge = Mock.Create<GraphEdge>();
            edge.Arrange(e => e.To).Returns(toNode);
            node.AddOutgoingEdge(edge);

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