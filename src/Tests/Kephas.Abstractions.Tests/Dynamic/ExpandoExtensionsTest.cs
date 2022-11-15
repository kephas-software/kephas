// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the expando extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Kephas.Dynamic;
    using NUnit.Framework;

    [TestFixture]
    public class ExpandoExtensionsTest
    {
        [Test]
        public void Merge_obj_in_null()
        {
            Assert.Throws<ArgumentNullException>(() => ((IDynamic)null!).Merge("1234"));
        }

        [Test]
        public void Merge_obj_in_expando()
        {
            var expando = new Expando();
            var result = expando.Merge("1234");

            Assert.AreSame(result, expando);
            Assert.AreEqual(expando["Length"], 4);
        }

        [Test]
        public void Merge_readonly_obj_in_expando()
        {
            var expando = new ExpandoWithReadonlyProperties("gigi") { Age = 20 };
            var result = expando.Merge(new ExpandoWithReadonlyProperties("belogea") { Age = 30 });

            Assert.AreEqual("gigi", result.Name);
            Assert.AreEqual(30, result.Age);
        }

        [Test]
        public void Merge_expando_no_private()
        {
            var expando = new Expando { ["my"] = "name" };
            var result = expando.Merge(new Expando { ["_is"] = "belogea" });

            Assert.AreEqual("name", result["my"]);
            Assert.IsNull(result["_is"]);
        }

        [Test]
        public void Merge_dictionary_of_string_in_expando()
        {
            var expando = new Expando();
            var result = expando.Merge(new Dictionary<string, string> { { "hi", "there" }, { "how", "are" }, { "you", "today" } });

            Assert.AreSame(result, expando);
            Assert.AreEqual("there", expando["hi"]);
            Assert.AreEqual("are", expando["how"]);
            Assert.AreEqual("today", expando["you"]);
        }

        [Test]
        public void Merge_dictionary_of_string_in_expando_no_private()
        {
            var expando = new Expando();
            var result = expando.Merge(new Dictionary<string, string> { { "_hi", "there" }, { "how", "are" }, { "you", "today" } });

            Assert.AreSame(result, expando);
            Assert.IsNull(expando["_hi"]);
            Assert.AreEqual("are", expando["how"]);
            Assert.AreEqual("today", expando["you"]);
        }

        [Test]
        public void Merge_dictionary_of_int_in_expando()
        {
            var expando = new Expando();
            var result = expando.Merge(new Dictionary<string, int> { { "hi", 1 }, { "how", 2 }, { "you", 3 } });

            Assert.AreSame(result, expando);
            Assert.AreEqual(1, expando["hi"]);
            Assert.AreEqual(2, expando["how"]);
            Assert.AreEqual(3, expando["you"]);
        }

        [Test]
        public void Merge_list_of_key_value_pairs_in_expando()
        {
            var expando = new Expando();
            var result = expando.Merge(new Dictionary<string, int> { { "hi", 1 }, { "how", 2 }, { "you", 3 } }.ToList());

            Assert.AreSame(result, expando);
            Assert.AreEqual(1, expando["hi"]);
            Assert.AreEqual(2, expando["how"]);
            Assert.AreEqual(3, expando["you"]);
        }

        [Test]
        public void GetLaxValue_string()
        {
            var expando = new Expando { ["myWay"] = "Frank Sinatra" };
            var actual = expando.GetLaxValue<string>("MyWay");

            Assert.AreEqual("Frank Sinatra", actual);
        }

        [Test]
        public void GetLaxValue_long_as_string()
        {
            var expando = new Expando { ["costs"] = "120000" };
            var actual = expando.GetLaxValue<long>("Costs");

            Assert.AreEqual(120000L, actual);
        }

        public class ExpandoWithReadonlyProperties : Expando
        {
            public ExpandoWithReadonlyProperties(string name)
            {
                this.Name = name;
            }

            public string Name { get; }

            public int Age { get; set; }
        }
    }
}