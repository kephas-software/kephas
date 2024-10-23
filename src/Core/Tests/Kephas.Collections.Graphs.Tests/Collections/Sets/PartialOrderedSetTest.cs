// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartialOrderedSetTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the partial ordered set test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Collections.Sets;
using Xunit;

namespace Kephas.Tests.Collections.Sets;

public class PartialOrderedSetTest
{
    [Fact]
    public void GetEnumerator_fully_ordered()
    {
        var orderedSet = new PartialOrderedSet<int>(new[] { 2, 3, 1 }, (i1, i2) => i1 - i2);
        var orderedValues = orderedSet.ToList();
        Assert.Equal(1, orderedValues[0]);
        Assert.Equal(2, orderedValues[1]);
        Assert.Equal(3, orderedValues[2]);
    }

    [Fact]
    public void GetEnumerator_not_ordered()
    {
        var orderedSet = new PartialOrderedSet<int>(new[] { 2, 3, 1 }, (i1, i2) => null);
        var orderedValues = orderedSet.ToList();
        Assert.Equal(2, orderedValues[0]);
        Assert.Equal(3, orderedValues[1]);
        Assert.Equal(1, orderedValues[2]);
    }

    [Fact]
    public void GetEnumerator_partially_ordered()
    {
        var orderedSet = new PartialOrderedSet<int>(new[] { 2, 3, 1 }, (i1, i2) => i1 + i2 == 5 ? (int?)i2 - i1 : null);
        var orderedValues = orderedSet.ToList();
        Assert.Equal(3, orderedValues[0]);
        Assert.Equal(2, orderedValues[1]);
        Assert.Equal(1, orderedValues[2]);
    }

    [Fact]
    public void GetEnumerator_cyclic_graph_exception()
    {
        Assert.Throws<InvalidOperationException>(
            () =>
            {
                var orderedSet = new PartialOrderedSet<int>(new[] { 1, 2 }, (i1, i2) => Math.Abs(i1 - i2));
            });
    }

    [Fact]
    public void Compare_direct_comparable()
    {
        var orderedSet = new PartialOrderedSet<int>(new[] { 2, 3, 1 }, (i1, i2) =>
        {
            if (i1 == 1 && i2 == 2 || i1 == 2 && i2 == 3)
            {
                return i1 - i2;
            }

            return null;
        });

        Assert.Equal(0, orderedSet.Compare(2, 2));
        Assert.Equal(-1, orderedSet.Compare(1, 2));
        Assert.Equal(1, orderedSet.Compare(3, 2));
    }

    [Fact]
    public void Compare_indirect_comparable()
    {
        var orderedSet = new PartialOrderedSet<int>(new[] { 2, 3, 1 }, (i1, i2) =>
        {
            if (i1 == 1 && i2 == 2 || i1 == 2 && i2 == 3)
            {
                return i1 - i2;
            }

            return null;
        });

        Assert.Equal(0, orderedSet.Compare(2, 2));
        Assert.Equal(-1, orderedSet.Compare(1, 3));
        Assert.Equal(1, orderedSet.Compare(3, 1));
    }

    [Fact]
    public void Compare_not_comparable()
    {
        var orderedSet = new PartialOrderedSet<int>(new[] { 2, 3, 1 }, (i1, i2) =>
        {
            if (i1 == 1 && i2 == 2)
            {
                return i1 - i2;
            }

            return null;
        });

        Assert.Null(orderedSet.Compare(1, 3));
        Assert.Null(orderedSet.Compare(2, 3));
    }

    [Fact]
    public void Compare_value_not_contained()
    {
        var orderedSet = new PartialOrderedSet<int>(new[] { 2, 3, 1 }, (i1, i2) => i1 - i2);

        Assert.Null(orderedSet.Compare(1, 4));
    }
}