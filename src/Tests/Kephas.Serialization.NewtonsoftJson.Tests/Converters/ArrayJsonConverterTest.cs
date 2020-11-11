// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayJsonConverterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests.Converters
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using NUnit.Framework;

    [TestFixture]
    public class ArrayJsonConverterTest : SerializationTestBase
    {
        [Test]
        public async Task SerializeAsync_Array()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new object?[] { "one", 2, null };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"[""one"",2,null]", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_Array()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            // var obj = new object?[] { "one", 2, null };
            var obj = await serializer.DeserializeAsync(
                @"[""one"",2,null]",
                this.GetSerializationContext(typeof(object[])));

            Assert.IsInstanceOf<object[]>(obj);
            var list = (object[])obj;
            Assert.AreEqual("one", list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.IsNull(list[2]);
        }

        [Test]
        public async Task SerializeAsync_Array_nested()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new NestedValues
            {
                Values = new object?[] { "one", 2, null },
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ArrayJsonConverterTest+NestedValues"",""values"":[""one"",2,null],""entities"":[]}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_Array_nested()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            // var obj = new object?[] { "one", 2, null };
            var obj = await serializer.DeserializeAsync(
                @"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ArrayJsonConverterTest+NestedValues"",""values"":[""one"",2,null]}");

            Assert.IsInstanceOf<NestedValues>(obj);
            var nestedValues = (NestedValues)obj;
            Assert.AreEqual("one", nestedValues.Values[0]);
            Assert.AreEqual(2, nestedValues.Values[1]);
            Assert.IsNull(nestedValues.Values[2]);
        }

        [Test]
        public async Task SerializeAsync_Array_nested_entities()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new NestedValues
            {
                Entities = new[] { new TestEntity { Name = "hi" }, new TestEntity { Name = "there" } },
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ArrayJsonConverterTest+NestedValues"",""values"":null,""entities"":[{""$type"":""Kephas.Serialization.Json.Tests.Converters.ArrayJsonConverterTest+TestEntity"",""name"":""hi"",""personalSite"":null},{""$type"":""Kephas.Serialization.Json.Tests.Converters.ArrayJsonConverterTest+TestEntity"",""name"":""there"",""personalSite"":null}]}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_Array_nested_entities()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            // var obj = new object?[] { "one", 2, null };
            var obj = await serializer.DeserializeAsync(
                @"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ArrayJsonConverterTest+NestedValues"",""values"":null,""entities"":[{""$type"":""Kephas.Serialization.Json.Tests.Converters.ArrayJsonConverterTest+TestEntity"",""name"":""hi"",""personalSite"":null},{""$type"":""Kephas.Serialization.Json.Tests.Converters.ArrayJsonConverterTest+TestEntity"",""name"":""there"",""personalSite"":null}]}");

            Assert.IsInstanceOf<NestedValues>(obj);
            var nestedValues = (NestedValues)obj;
            Assert.AreEqual("hi", nestedValues.Entities[0].Name);
            Assert.AreEqual("there", nestedValues.Entities[1].Name);
            Assert.IsNull(nestedValues.Values);
        }

        public class TestEntity
        {
            public string Name { get; set; }

            public Uri PersonalSite { get; set; }
        }

        public class NestedValues
        {
            public object?[] Values { get; set; }

            public TestEntity[] Entities { get; set; } = new TestEntity[0];
        }
    }
}