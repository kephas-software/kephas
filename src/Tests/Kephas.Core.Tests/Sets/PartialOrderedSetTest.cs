// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartialOrderedSetTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the partial ordered set test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Sets
{
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
    }
}