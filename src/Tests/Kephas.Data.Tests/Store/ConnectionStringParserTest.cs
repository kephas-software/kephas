// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStringParserTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the connection string parser test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Store
{
    using Kephas.Data.Store;

    using NUnit.Framework;

    [TestFixture]
    public class ConnectionStringParserTest
    {
        [Test]
        public void AsDictionary_common_case()
        {
            var values = ConnectionStringParser.AsDictionary("Key1=val1;Key2=val2");

            Assert.AreEqual(2, values.Count);
            Assert.AreEqual("val1", values["Key1"]);
            Assert.AreEqual("val2", values["Key2"]);
        }

        [Test]
        public void AsDictionary_ignore_empty_entries()
        {
            var values = ConnectionStringParser.AsDictionary("Key1=val1;;Key2=val2");

            Assert.AreEqual(2, values.Count);
            Assert.AreEqual("val1", values["Key1"]);
            Assert.AreEqual("val2", values["Key2"]);
        }

        [Test]
        public void AsDictionary_missing_param_value()
        {
            var values = ConnectionStringParser.AsDictionary("Key1=val1;Key2");

            Assert.AreEqual(1, values.Count);
            Assert.AreEqual("val1", values["Key1"]);
        }

        [Test]
        public void AsDictionary_param_value_is_empty()
        {
            var values = ConnectionStringParser.AsDictionary("Key1=val1;Key2=");

            Assert.AreEqual(2, values.Count);
            Assert.AreEqual("val1", values["Key1"]);
            Assert.AreEqual(string.Empty, values["Key2"]);
        }

        [Test]
        public void AsExpando_common_case()
        {
            dynamic values = ConnectionStringParser.AsExpando("Key1=val1;Key2=val2");

            Assert.AreEqual("val1", values.Key1);
            Assert.AreEqual("val2", values.Key2);
        }

        [Test]
        public void AsExpando_ignore_empty_entries()
        {
            dynamic values = ConnectionStringParser.AsExpando("Key1=val1;;Key2=val2");

            Assert.AreEqual("val1", values.Key1);
            Assert.AreEqual("val2", values.Key2);
        }

        [Test]
        public void AsExpando_missing_param_value()
        {
            dynamic values = ConnectionStringParser.AsExpando("Key1=val1;Key2");

            Assert.AreEqual("val1", values.Key1);
        }

        [Test]
        public void AsExpando_param_value_is_empty()
        {
            dynamic values = ConnectionStringParser.AsExpando("Key1=val1;Key2=");

            Assert.AreEqual("val1", values.Key1);
            Assert.AreEqual(string.Empty, values.Key2);
        }
    }
}