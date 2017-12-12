﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the expando base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Dynamic
{
    using System.Collections.Generic;

    using Kephas.Dynamic;

    using NUnit.Framework;

    [TestFixture]
    public class ExpandoBaseTest
    {
        [Test]
        public void Constructor_same_object_and_dictionary()
        {
            var dict = new Dictionary<string, object>();
            dynamic expando = new TestExpando(dict, dict);
            Assert.IsNull(expando.Count);
        }

        [Test]
        public void ToDictionary_duplicated_property()
        {
            var dict = new Dictionary<string, object>();
            var expando = new TestExpando(dict);
            dict["Name"] = "gigi";
            expando.Name = "belogea";

            var dictionary = expando.ToDictionary();
            Assert.AreEqual("belogea", dictionary["Name"]);
        }

        [Test]
        public void ToDictionary_wrapped_duplicated_property()
        {
            var dict = new Dictionary<string, object>();
            var obj = new Named();
            var expando = new TestExpando(obj, dict);
            dict["Name"] = "gigi";
            expando.Name = "belogea";
            obj.Name = "rules";

            var dictionary = expando.ToDictionary();
            Assert.AreEqual("belogea", dictionary["Name"]);
        }
    }

    public class TestExpando : ExpandoBase
    {
        public TestExpando(object inner, IDictionary<string, object> innerDictionary = null)
            : base(inner, innerDictionary)
        {
        }

        public string Name { get; set; }
    }

    public class Named
    {
        public string Name { get; set; }
    }
}