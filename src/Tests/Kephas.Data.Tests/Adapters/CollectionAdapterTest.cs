// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionAdapterTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    }
}