namespace Kephas.Core.Tests.Graphs
{
    using Kephas.Graphs;

    using NUnit.Framework;

    [TestFixture]
    public class GraphNodeOfTValueTest
    {
        [Test]
        public void ToString_success()
        {
            var node = new GraphNode<int> { Value = 3 };
            var toString = node.ToString();
            Assert.AreEqual("{3}", toString);
        }
    }
}