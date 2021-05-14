// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphEdgeOfTNodeValueTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
            var edge = new GraphEdge<int, int>(new GraphNode<int>(1), new GraphNode<int>(2));
            var toString = edge.ToString();
            Assert.AreEqual("{1} -> {2}", toString);
        }
    }
}