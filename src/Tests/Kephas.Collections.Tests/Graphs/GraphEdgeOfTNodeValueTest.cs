namespace Kephas.Core.Tests.Graphs
{
    using Kephas.Graphs;

    using NUnit.Framework;

    [TestFixture]
    public class GraphEdgeOfTNodeValueTest
    {
        [Test]
        public void ToString_success()
        {
            var edge = new GraphEdge<int>(new GraphNode<int>(1), new GraphNode<int>(2));
            var toString = edge.ToString();
            Assert.AreEqual("{1} -> {2}", toString);
        }
    }
}