// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicElementInfoCollectionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Reflection.Dynamic
{
    using System;

    using Kephas.Reflection;
    using Kephas.Reflection.Dynamic;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DynamicElementInfoCollectionTest
    {
        [Test]
        public void Add_sets_declaring_container()
        {
            var container = Substitute.For<IElementInfo>();
            var item = new TestElementInfo();
            var col = new DynamicElementInfoCollection<IElementInfo>(container) { item };

            Assert.AreSame(container, item.DeclaringContainer);
        }

        [Test]
        public void Add_sets_position()
        {
            var container = Substitute.For<IElementInfo>();
            var item1 = new TestElementInfo();
            var item2 = new TestElementInfo();
            var col = new DynamicElementInfoCollection<IElementInfo>(container) { item1, item2 };

            Assert.AreEqual(0, item1.GetPosition());
            Assert.AreEqual(1, item2.GetPosition());
        }

        [Test]
        public void Add_item_requires_DynamicElementInfo()
        {
            var item = Substitute.For<IElementInfo>();
            var col = new DynamicElementInfoCollection<IElementInfo>(Substitute.For<IElementInfo>());

            Assert.Throws<InvalidOperationException>(() => col.Add(item));
        }

        private class TestElementInfo : DynamicElementInfo
        {
            public int GetPosition() => this.Position;
        }
    }
}