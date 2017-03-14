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
        public void Parse_common_case()
        {
            var values = ConnectionStringParser.Parse("Key1=val1;Key2=val2");

            Assert.AreEqual(2, values.Count);
            Assert.AreEqual("val1", values["Key1"]);
            Assert.AreEqual("val2", values["Key2"]);
        }

        [Test]
        public void Parse_ignore_empty_entries()
        {
            var values = ConnectionStringParser.Parse("Key1=val1;;Key2=val2");

            Assert.AreEqual(2, values.Count);
            Assert.AreEqual("val1", values["Key1"]);
            Assert.AreEqual("val2", values["Key2"]);
        }

        [Test]
        public void Parse_missing_param_value()
        {
            var values = ConnectionStringParser.Parse("Key1=val1;Key2");

            Assert.AreEqual(1, values.Count);
            Assert.AreEqual("val1", values["Key1"]);
        }

        [Test]
        public void Parse_param_value_is_empty()
        {
            var values = ConnectionStringParser.Parse("Key1=val1;Key2=");

            Assert.AreEqual(2, values.Count);
            Assert.AreEqual("val1", values["Key1"]);
            Assert.AreEqual(string.Empty, values["Key2"]);
        }
    }
}