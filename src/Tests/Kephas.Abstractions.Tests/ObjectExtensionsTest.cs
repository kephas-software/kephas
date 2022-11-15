// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Tests for <see cref="ObjectExtensions" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;

    using Kephas.Dynamic;
    using NSubstitute;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="DynamicObjectExtensions"/>
    /// </summary>
    [TestFixture]
    public class ObjectExtensionsTest
    {
        [Test]
        public void ToDynamicObject_dynamic()
        {
            var obj = new ExpandoObject();
            var dyn = obj.ToDynamicObject();
            Assert.AreSame(obj, dyn);
        }

        [Test]
        public void ToDynamicObject_non_dynamic_method()
        {
            var obj = new List<string>();
            var dyn = obj.ToDynamicObject();
            Assert.AreNotSame(obj, dyn);

            dyn.Add("John");
            Assert.AreEqual(1, obj.Count);
            Assert.IsTrue(obj.Contains("John"));
        }

        [Test]
        public void ToDynamicObject_non_dynamic_property()
        {
            var obj = new List<string> { "one", "two" };
            var dyn = obj.ToDynamicObject();
            Assert.AreNotSame(obj, dyn);

            Assert.AreEqual(2, dyn.Count);
        }

        [Test]
        public void ToDynamicObject_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => ((object)null).ToDynamicObject());
        }

        [Test]
        public void ToDynamic_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => ((object)null!).ToDynamic());
        }

        [Test]
        public void ToDynamic_object()
        {
            var obj = new { Name = "John", FamilyName = "Doe" };
            var expando = obj.ToDynamic();

            Assert.AreNotSame(obj, expando);
            Assert.AreEqual("John", expando["Name"]);
            Assert.AreEqual("Doe", expando["FamilyName"]);
        }

        [Test]
        public void ToDynamic_dynamic()
        {
            var expando = Substitute.For<IDynamic>();
            Assert.AreSame(expando, expando.ToDynamic());
        }

        [Test]
        public void ToDynamic_dictionary_string_object()
        {
            var dictionary = new Dictionary<string, object?>();
            var expando = dictionary.ToDynamic();

            expando["hi"] = "there";

            Assert.AreEqual("there", dictionary["hi"]);
        }

        [Test]
        public void ToDynamic_dictionary_string_string()
        {
            var dictionary = new Dictionary<string, string>();
            var expando = dictionary.ToDynamic();

            expando["hi"] = "there";

            Assert.AreEqual("there", dictionary["hi"]);
        }
    }
}