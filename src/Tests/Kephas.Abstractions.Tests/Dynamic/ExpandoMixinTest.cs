// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoMixinTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Dynamic
{
    using System.Collections.Generic;
    using Kephas.Dynamic;
    using NUnit.Framework;

    [TestFixture]
    public class ExpandoMixinTest
    {
        [Test]
        public void Indexer_from_dictionary()
        {
            var expando = new Expandable();
            expando.ToDynamic()["Hi"] = "there";
            Assert.AreEqual("there", expando.ToDynamic()["Hi"]);
        }

        [Test]
        public void Indexer_from_member()
        {
            var expando = new Expandable();
            expando.ToDynamic()["Name"] = "John Doe";
            Assert.AreEqual("John Doe", expando.ToDynamic()["Name"]);
            Assert.AreEqual("John Doe", expando.Name);
        }

        [Test]
        public void HasDynamicMember()
        {
            var expando = new Expandable();
            expando.ToDynamic()["Hi"] = "there";
            Assert.IsTrue(expando.ToDynamic().HasDynamicMember("Hi"));
            Assert.IsFalse(expando.ToDynamic().HasDynamicMember("there"));
        }

        [Test]
        public void ToDictionary()
        {
            var expando = new Expandable();
            expando.ToDynamic()["Hi"] = "there";

            var dict = expando.ToDynamic().ToDictionary();
            Assert.IsTrue(dict.ContainsKey("Hi"));
            Assert.IsFalse(dict.ContainsKey("there"));
        }

        public class Expandable : IExpandoMixin
        {
            private IDictionary<string, object?> inner = new Dictionary<string, object?>();

            IDictionary<string, object?> IExpandoMixin.InnerDictionary => this.inner;

            public string? Name { get; set; }
        }
    }
}