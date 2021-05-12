// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the type extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Reflection
{
    using System.Collections.Generic;

    using Kephas.Reflection;

    using NUnit.Framework;

    [TestFixture]
    public class TypeExtensionsTest
    {
        [Test]
        public void TryGetDictionaryKeyItemType_not_enumerable()
        {
            var itemType = typeof(int).TryGetDictionaryKeyItemType();
            Assert.IsNull(itemType);
        }

        [Test]
        public void TryGetDictionaryKeyItemType_closed_generic_dictionary()
        {
            var itemType = typeof(IDictionary<int, object?>).TryGetDictionaryKeyItemType();
            Assert.AreEqual(typeof(int), itemType.Value.keyType);
            Assert.AreEqual(typeof(object), itemType.Value.itemType);
        }

        [Test]
        public void TryGetDictionaryKeyItemType_closed_generic_dictionary_class()
        {
            var itemType = typeof(Dictionary<int, object?>).TryGetDictionaryKeyItemType();
            Assert.AreEqual(typeof(int), itemType.Value.keyType);
            Assert.AreEqual(typeof(object), itemType.Value.itemType);
        }

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


        [Test]
        public void TryGetEnumerableItemType_closed_generic_collection_list()
        {
            var itemType = typeof(List<string>).TryGetEnumerableItemType();
            Assert.AreEqual(typeof(string), itemType);
        }

        [Test]
        public void GetQualifiedFullName_version_info_stripped()
        {
            var type = typeof(string);
            var qualifiedFullName = type.GetQualifiedFullName(stripVersionInfo: true);
            Assert.AreEqual("System.String, System.Private.CoreLib", qualifiedFullName);
        }

        [Test]
        public void GetQualifiedFullName_version_info_not_stripped()
        {
            var type = typeof(string);

            Assert.AreEqual(type.AssemblyQualifiedName, type.GetQualifiedFullName(stripVersionInfo: false));
        }

        [Test]
        public void GetBaseConstructedGenericOf_interface()
        {
            var type = typeof(string);
            var constructedGenericType = type.GetBaseConstructedGenericOf(typeof(IEnumerable<>));

            Assert.AreSame(constructedGenericType, typeof(IEnumerable<char>));
        }

        [Test]
        public void GetBaseConstructedGenericOf_class()
        {
            var type = typeof(TestDerivedGeneric<string>);
            var constructedGenericType = type.GetBaseConstructedGenericOf(typeof(TestGeneric<>));

            Assert.AreSame(constructedGenericType, typeof(TestGeneric<string>));
        }
        
        [Test]
        public void IsCollection_direct()
        {
            var type = typeof(ICollection<string>);

            Assert.IsTrue(type.IsCollection());
        }
        
        [Test]
        public void IsCollection_indirect()
        {
            var type = typeof(List<string>);

            Assert.IsTrue(type.IsCollection());
        }
        
        [Test]
        public void IsDictionary_direct()
        {
            var type = typeof(IDictionary<string, object>);

            Assert.IsTrue(type.IsDictionary());
        }
        
        [Test]
        public void IsDictionary_indirect()
        {
            var type = typeof(Dictionary<string, int>);

            Assert.IsTrue(type.IsDictionary());
        }

        public class TestGeneric<T> { }
        public class TestDerivedGeneric<T> : TestGeneric<T> { }
    }
}