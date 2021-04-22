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
        public void Add_item_requires_DynamicElementInfo()
        {
            var item = Substitute.For<IElementInfo>();
            var col = new DynamicElementInfoCollection<IElementInfo>(Substitute.For<IElementInfo>());

            Assert.Throws<InvalidOperationException>(() => col.Add(item));
        }

        private class TestElementInfo : DynamicElementInfo { }
    }
}