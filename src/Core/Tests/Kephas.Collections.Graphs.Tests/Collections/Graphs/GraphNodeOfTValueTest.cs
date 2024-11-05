// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphNodeOfTValueTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Collections.Graphs;
using Xunit;

namespace Kephas.Tests.Collections.Graphs;

public class GraphNodeOfTValueTest
{
    [Fact]
    public void ToString_success()
    {
        var node = new GraphNode<int> { Value = 3 };
        var toString = node.ToString();
        Assert.Equal("{3}", toString);
    }
}