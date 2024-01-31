// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphEdgeOfTNodeValueTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Collections.Graphs;
using Xunit;

namespace Kephas.Tests.Collections.Graphs
{
        public class GraphEdgeOfTNodeValueTest
    {
        [Fact]
        public void ToString_success()
        {
            var edge = new GraphEdge<int, int>(new GraphNode<int>(1), new GraphNode<int>(2));
            var toString = edge.ToString();
            Assert.Equal("{1} -> {2}", toString);
        }
    }
}
