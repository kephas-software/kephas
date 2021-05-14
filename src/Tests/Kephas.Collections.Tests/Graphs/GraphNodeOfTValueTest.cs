// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphNodeOfTValueTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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