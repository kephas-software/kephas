// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Collections
{
    using System.Collections.Generic;

    using Kephas.Collections;
    using NUnit.Framework;

    [TestFixture]
    public class DictionaryExtensionsTest
    {
        [Test]
        public void Merge_different()
        {
            var dict = new Dictionary<string, string> { { "1", "one" } }
                .Merge(new Dictionary<string, string>() { { "2", "two" } });

            Assert.AreEqual(2, dict.Count);
            Assert.AreEqual("one", dict["1"]);
            Assert.AreEqual("two", dict["2"]);
        }

        [Test]
        public void Merge_same_key()
        {
            var dict = new Dictionary<string, string> { { "1", "one" }, { "2", "two" } }
                .Merge(new Dictionary<string, string>() { { "2", "three" } });

            Assert.AreEqual(2, dict.Count);
            Assert.AreEqual("one", dict["1"]);
            Assert.AreEqual("three", dict["2"]);
        }
    }
}