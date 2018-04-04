// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionAdapterTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the entity collection adapter test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Adapters
{
    using System;
    using System.Collections.Generic;

    using Kephas.Data.Adapters;

    using NUnit.Framework;

    [TestFixture]
    public class CollectionAdapterTest
    {
        [Test]
        public void GetEnumerator()
        {
            var col = new List<string> { "123", "234" };
            var newCol = new List<IConvertible>();
            var adapter = new CollectionAdapter<string, IConvertible>(col);
            using (var enumerator = adapter.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    newCol.Add(enumerator.Current);
                }
            }

            Assert.AreEqual(2, newCol.Count);
        }

        [Test]
        public void CopyTo()
        {
            var col = new List<string> { "123", "234" };
            var adapter = new CollectionAdapter<string, IConvertible>(col);

            var convArray = new IConvertible[10];
            adapter.CopyTo(convArray, 2);

            Assert.AreEqual(convArray[2], "123");
            Assert.AreEqual(convArray[3], "234");
        }

        [Test]
        public void CopyTo_ArgumentOutOfRange()
        {
            var col = new List<string> { "123", "234" };
            var adapter = new CollectionAdapter<string, IConvertible>(col);

            var convArray = new IConvertible[1];
            Assert.Throws<ArgumentException>(() => adapter.CopyTo(convArray, 2));
        }
    }
}