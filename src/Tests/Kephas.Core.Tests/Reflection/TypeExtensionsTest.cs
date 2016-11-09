namespace Kephas.Core.Tests.Reflection
{
    using System.Collections;
    using System.Collections.Generic;

    using Kephas.Reflection;

    using NUnit.Framework;

    [TestFixture]
    public class TypeExtensionsTest
    {
        [Test]
        public void TryGetEnumerableItemType_not_enumerable()
        {
            var itemType = typeof(int).TryGetEnumerableItemType();
            Assert.IsNull(itemType);
        }

        [Test]
        public void TryGetEnumerableItemType_closed_generic_collection()
        {
            var itemType = typeof(ICollection<string>).TryGetEnumerableItemType();
            Assert.AreEqual(typeof(string), itemType);
        }
    }
}