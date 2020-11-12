// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryJsonConverterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using NUnit.Framework;

    [TestFixture]
    public class DictionaryJsonConverterTest : SerializationTestBase
    {
        [Test]
        public async Task SerializeAsync_Dictionary_preserve_case()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new Dictionary<string, object>
            {
                ["Description"] = "John Doe",
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""Description"":""John Doe""}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_Dictionary_preserve_case()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(
                @"{""description"":""John Doe""}",
                this.GetSerializationContext(typeof(Dictionary<string, object>)));

            Assert.IsInstanceOf<Dictionary<string, object>>(obj);
            var dict = (Dictionary<string, object>)obj;
            CollectionAssert.IsNotEmpty(dict);
            Assert.AreEqual("John Doe", dict["description"]);
        }

        [Test]
        public async Task DeserializeAsync_IDictionary()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(
                @"{""description"":""John Doe""}",
                this.GetSerializationContext(typeof(IDictionary<string, object>)));

            Assert.IsInstanceOf<Dictionary<string, object>>(obj);
            var dict = (Dictionary<string, object>)obj;
            CollectionAssert.IsNotEmpty(dict);
            Assert.AreEqual("John Doe", dict["description"]);
        }

        [Test]
        public async Task SerializeAsync_Dictionary_nested()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new NestedValues
            {
                Values =
                {
                    ["Description"] = "John Doe",
                },
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+NestedValues"",""values"":{""Description"":""John Doe""},""entities"":null}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_Dictionary_nested()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(
                @"{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+NestedValues"",""values"":{""Description"":""John Doe""}}");

            Assert.IsInstanceOf<NestedValues>(obj);
            var nested = (NestedValues)obj;
            Assert.IsNull(nested.Entities);
            Assert.AreEqual("John Doe", nested.Values["Description"]);
        }

        [Test]
        public async Task SerializeAsync_Dictionary_nested_object_item()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new NestedValues
            {
                Values =
                {
                    ["Customer"] = new TestEntity { Name = "gigi" },
                },
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+NestedValues"",""values"":{""Customer"":{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+TestEntity"",""name"":""gigi"",""personalSite"":null}},""entities"":null}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_Dictionary_nested_object_item()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(
                @"{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+NestedValues"",""values"":{""Customer"":{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+TestEntity"",""name"":""gigi"",""personalSite"":null}},""entities"":null}");

            Assert.IsInstanceOf<NestedValues>(obj);
            var nested = (NestedValues)obj;
            Assert.IsNull(nested.Entities);
            Assert.IsInstanceOf<TestEntity>(nested.Values["Customer"]);
            Assert.AreEqual("gigi", (nested.Values["Customer"] as TestEntity).Name);
        }

        [Test]
        public async Task SerializeAsync_Dictionary_entities()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new NestedValues
            {
                Entities = new Dictionary<string, TestEntity>
                {
                    ["Customer"] = new TestEntity { Name = "gigi" },
                },
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+NestedValues"",""values"":{},""entities"":{""Customer"":{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+TestEntity"",""name"":""gigi"",""personalSite"":null}}}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_Dictionary_Entities()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(
                @"{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+NestedValues"",""entities"":{""Customer"":{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+TestEntity"",""name"":""gigi"",""personalSite"":null}}}");

            Assert.IsInstanceOf<NestedValues>(obj);
            var nested = (NestedValues)obj;
            Assert.IsNotNull(nested.Entities);
            Assert.IsInstanceOf<TestEntity>(nested.Entities["Customer"]);
            Assert.AreEqual("gigi", nested.Entities["Customer"].Name);
        }

        [Test]
        public async Task DeserializeAsync_Dictionary_Entities_no_types()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(
                @"{""entities"":{""Customer"":{""name"":""gigi"",""personalSite"":null}}}",
                this.GetSerializationContext(typeof(NestedValues)));

            Assert.IsInstanceOf<NestedValues>(obj);
            var nested = (NestedValues)obj;
            Assert.IsNotNull(nested.Entities);
            Assert.IsInstanceOf<TestEntity>(nested.Entities["Customer"]);
            Assert.AreEqual("gigi", nested.Entities["Customer"].Name);
        }

        [Test]
        public async Task SerializeAsync_MyDictionary_entities()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new NestedValues
            {
                Entities = new MyDictionary
                {
                    ["Customer"] = new TestEntity { Name = "gigi" },
                },
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+NestedValues"",""values"":{},""entities"":{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+MyDictionary"",""Customer"":{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+TestEntity"",""name"":""gigi"",""personalSite"":null}}}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_MyDictionary_Entities()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(
                @"{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+NestedValues"",""values"":{},""entities"":{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+MyDictionary"",""Customer"":{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+TestEntity"",""name"":""gigi"",""personalSite"":null}}}");

            Assert.IsInstanceOf<NestedValues>(obj);
            var nested = (NestedValues)obj;
            Assert.IsInstanceOf<MyDictionary>(nested.Entities);
            Assert.IsInstanceOf<TestEntity>(nested.Entities["Customer"]);
            Assert.AreEqual("gigi", nested.Entities["Customer"].Name);
        }

        public class TestEntity
        {
            public string Name { get; set; }

            public Uri PersonalSite { get; set; }
        }

        public class NestedValues
        {
            public IDictionary<string, object> Values { get; } = new Dictionary<string, object>();

            public IDictionary<string, TestEntity> Entities { get; set; }
        }

        public class MyDictionary : Dictionary<string, TestEntity>
        {
        }
    }
}