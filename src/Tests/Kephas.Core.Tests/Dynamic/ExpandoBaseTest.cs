// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the expando base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        public void GetDynamicMemberNames_no_duplicates()
        {
            var expando = new TestExpando(new Named());

            var members = expando.GetDynamicMemberNames().ToList();
            Assert.AreEqual(1, members.Count);
            Assert.AreEqual("Name", members[0]);
        }

        [Test]
        public void GetDynamicMemberNames_this_binders()
        {
            var expando = new TestExpando(new object(), binders: ExpandoMemberBinderKind.This);
            expando["Name"] = "gigi";
            expando["FamilyName"] = "belogea";

            var members = expando.GetDynamicMemberNames().ToList();
            Assert.AreEqual(1, members.Count);
            Assert.AreEqual("Name", members[0]);
        }

        [Test]
        public void GetDynamicMemberNames_inner_object_binders()
        {
            var obj = new Named();
            var expando = new TestExpando(obj, binders: ExpandoMemberBinderKind.InnerObject);
            expando["Name"] = "gigi";
            expando["FamilyName"] = "belogea";

            var members = expando.GetDynamicMemberNames().ToList();
            Assert.AreEqual(1, members.Count);
            Assert.AreEqual("Name", members[0]);
        }

        [Test]
        public void GetDynamicMemberNames_inner_dictionary_binders()
        {
            var obj = new Named();
            var expando = new TestExpando(obj, binders: ExpandoMemberBinderKind.InnerDictionary);
            expando["FamilyName"] = "belogea";

            var members = expando.GetDynamicMemberNames().ToList();
            Assert.AreEqual(1, members.Count);
            Assert.AreEqual("FamilyName", members[0]);
        }

        [Test]
        public void HasDynamicMember_this_binders()
        {
            var expando = new TestExpando(new object(), binders: ExpandoMemberBinderKind.This);
            expando["Name"] = "gigi";
            expando["FamilyName"] = "belogea";

            Assert.IsTrue(expando.HasDynamicMember("Name"));
            Assert.IsFalse(expando.HasDynamicMember("FamilyName"));
        }

        [Test]
        public void HasDynamicMember_inner_object_binders()
        {
            var obj = new Named();
            var expando = new TestExpando(obj, binders: ExpandoMemberBinderKind.InnerObject);
            expando["Name"] = "gigi";
            expando["FamilyName"] = "belogea";

            Assert.IsTrue(expando.HasDynamicMember("Name"));
            Assert.IsFalse(expando.HasDynamicMember("FamilyName"));
        }

        [Test]
        public void HasDynamicMember_inner_dictionary_binders()
        {
            var obj = new Named();
            var expando = new TestExpando(obj, binders: ExpandoMemberBinderKind.InnerDictionary);
            expando["FamilyName"] = "belogea";

            Assert.IsFalse(expando.HasDynamicMember("Name"));
            Assert.IsTrue(expando.HasDynamicMember("FamilyName"));
        }

        [Test]
        public void ToDictionary_this_binders()
        {
            var expando = new TestExpando(new object(), binders: ExpandoMemberBinderKind.This);
            expando["Name"] = "gigi";
            expando["FamilyName"] = "belogea";

            var dictionary = expando.ToDictionary();
            Assert.AreEqual(1, dictionary.Count);
            Assert.AreEqual("gigi", dictionary["Name"]);
            Assert.AreEqual("gigi", expando.Name);
            Assert.IsFalse(expando.GetInnerDictionary().ContainsKey("Name"));
        }

        [Test]
        public void ToDictionary_inner_object_binders()
        {
            var obj = new Named();
            var expando = new TestExpando(obj, binders: ExpandoMemberBinderKind.InnerObject);
            expando["Name"] = "gigi";
            expando["FamilyName"] = "belogea";

            var dictionary = expando.ToDictionary();
            Assert.AreEqual(1, dictionary.Count);
            Assert.AreEqual("gigi", dictionary["Name"]);
            Assert.AreEqual("gigi", obj.Name);
            Assert.IsNull(expando.Name);
        }

        [Test]
        public void ToDictionary_inner_dictionary_binders()
        {
            var obj = new Named();
            var expando = new TestExpando(obj, binders: ExpandoMemberBinderKind.InnerDictionary);
            expando["Name"] = "gigi";
            expando["FamilyName"] = "belogea";

            var dictionary = expando.ToDictionary();
            Assert.AreEqual(2, dictionary.Count);
            Assert.AreEqual("gigi", dictionary["Name"]);
            Assert.AreEqual("belogea", dictionary["FamilyName"]);
            Assert.IsNull(obj.Name);
            Assert.IsNull(expando.Name);
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

        [Test]
        public void ToDictionary_transformed_key()
        {
            var dict = new Dictionary<string, object>();
            var expando = new TestExpando(dict);
            dict["Name"] = "gigi";
            expando.Name = "belogea";

            var dictionary = expando.ToDictionary(k => k.ToLower());
            Assert.AreEqual("belogea", dictionary["name"]);
        }

        [Test]
        public void ToDictionary_transformed_value()
        {
            var dict = new Dictionary<string, object>();
            var expando = new TestExpando(dict);
            dict["Name"] = "gigi";
            expando.Name = "belogea";

            var dictionary = expando.ToDictionary(valueFunc: v => v is string stringValue ? stringValue.ToUpper() : v);
            Assert.AreEqual("BELOGEA", dictionary["Name"]);
        }

        [Test]
        public void ToDictionary_transformed_key_and_value()
        {
            var dict = new Dictionary<string, object>();
            var expando = new TestExpando(dict);
            dict["Name"] = "gigi";
            expando.Name = "belogea";

            var dictionary = expando.ToDictionary(k => k.ToLower(), v => v is string stringValue ? stringValue.ToUpper() : v);
            Assert.AreEqual("BELOGEA", dictionary["name"]);
        }

        [Test]
        public void Indexer_non_conflicting_with_constant()
        {
            var dict = new Dictionary<string, object>();
            var expando = new TestExpandoWithConstants(dict);
            expando[nameof(TestExpandoWithConstants.Constant)] = "something";

            Assert.AreEqual("something", expando[nameof(TestExpandoWithConstants.Constant)]);
        }
    }

    public class TestExpandoWithConstants : ExpandoBase
    {
        public const string Constant = "gigi";

        public TestExpandoWithConstants(object inner, IDictionary<string, object> innerDictionary = null)
            : base(inner, innerDictionary)
        {
        }

        public string Name { get; set; }
    }

    public class TestExpando : ExpandoBase
    {
        public TestExpando(object inner, IDictionary<string, object> innerDictionary = null, ExpandoMemberBinderKind? binders = null)
            : base(inner, innerDictionary)
        {
            if (binders != null)
            {
                this.MemberBinders = binders.Value;
            }
        }

        public string Name { get; set; }

        public IDictionary<string, object?> GetInnerDictionary() => this.InnerDictionary;
    }

    public class TestExpandoNonConflicting : ExpandoBase
    {
        public TestExpandoNonConflicting(object inner, IDictionary<string, object> innerDictionary = null)
            : base(inner, innerDictionary)
        {
        }
    }

    public class Named
    {
        public string Name { get; set; }
    }
}