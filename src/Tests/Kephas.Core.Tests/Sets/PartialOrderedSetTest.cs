// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartialOrderedSetTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the partial ordered set test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Sets
{
    using System;
    using System.Linq;

    using Kephas.Sets;

    using NUnit.Framework;

    [TestFixture]
    public class PartialOrderedSetTest
    {
        [Test]
        public void GetEnumerator_fully_ordered()
        {
            var orderedSet = new PartialOrderedSet<int>(new[] { 2, 3, 1 }, (i1, i2) => i1 - i2);
            var orderedValues = orderedSet.ToList();
            Assert.AreEqual(1, orderedValues[0]);
            Assert.AreEqual(2, orderedValues[1]);
            Assert.AreEqual(3, orderedValues[2]);
        }

        [Test]
        public void GetEnumerator_not_ordered()
        {
            var orderedSet = new PartialOrderedSet<int>(new[] { 2, 3, 1 }, (i1, i2) => null);
            var orderedValues = orderedSet.ToList();
            Assert.AreEqual(2, orderedValues[0]);
            Assert.AreEqual(3, orderedValues[1]);
            Assert.AreEqual(1, orderedValues[2]);
        }

        [Test]
        public void GetEnumerator_partially_ordered()
        {
            var orderedSet = new PartialOrderedSet<int>(new[] { 2, 3, 1 }, (i1, i2) => i1 + i2 == 5 ? (int?)i2 - i1 : null);
            var orderedValues = orderedSet.ToList();
            Assert.AreEqual(3, orderedValues[0]);
            Assert.AreEqual(2, orderedValues[1]);
            Assert.AreEqual(1, orderedValues[2]);
        }

        [Test]
        public void GetEnumerator_cyclic_graph_exception()
        {
            Assert.Throws<InvalidOperationException>(
                () =>
                    {
                        var orderedSet = new PartialOrderedSet<int>(new[] { 1, 2 }, (i1, i2) => Math.Abs(i1 - i2));
                    });
        }

        [Test]
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

            Assert.AreEqual(0, orderedSet.Compare(2, 2));
            Assert.AreEqual(-1, orderedSet.Compare(1, 2));
            Assert.AreEqual(1, orderedSet.Compare(3, 2));
        }

        [Test]
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

            Assert.AreEqual(0, orderedSet.Compare(2, 2));
            Assert.AreEqual(-1, orderedSet.Compare(1, 3));
            Assert.AreEqual(1, orderedSet.Compare(3, 1));
        }

        [Test]
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

            Assert.IsNull(orderedSet.Compare(1, 3));
            Assert.IsNull(orderedSet.Compare(2, 3));
        }
    }
}