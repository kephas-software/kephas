// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionJsonConverterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests.Converters
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using NUnit.Framework;

    [TestFixture]
    public class CollectionJsonConverterTest : SerializationTestBase
    {
        [Test]
        public async Task SerializeAsync_List()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new List<object?> { "one", 2, null };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"[""one"",2,null]", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_List()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(
                @"[""one"",2,null]",
                this.GetSerializationContext(typeof(List<object>)));

            Assert.IsInstanceOf<List<object>>(obj);
            var list = (List<object>)obj;
            Assert.AreEqual("one", list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.IsNull(list[2]);
        }

        [Test]
        public async Task DeserializeAsync_ICollection()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(
                @"[""one"",2,null]",
                this.GetSerializationContext(typeof(ICollection<object>)));

            Assert.IsInstanceOf<List<object>>(obj);
            var list = (List<object>)obj;
            Assert.AreEqual("one", list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.IsNull(list[2]);
        }

        [Test]
        public async Task SerializeAsync_HashSet_nested_object()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new HashSet<object?> { "one", new TestEntity { Name = "gigi" } };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"[""one"",{""$type"":""Kephas.Serialization.Json.Tests.Converters.CollectionJsonConverterTest+TestEntity"",""name"":""gigi"",""personalSite"":null}]", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_HashSet_nested_object()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);

            var obj = await serializer.DeserializeAsync(
                @"[""one"",{""$type"":""Kephas.Serialization.Json.Tests.Converters.CollectionJsonConverterTest+TestEntity"",""name"":""gigi"",""personalSite"":null}]",
                this.GetSerializationContext(typeof(HashSet<object>)));

            Assert.IsInstanceOf<HashSet<object>>(obj);
            var list = ((HashSet<object>)obj).ToList();
            Assert.AreEqual("one", list[0]);
            Assert.IsInstanceOf<TestEntity>(list[1]);
        }

        [Test]
        public async Task SerializeAsync_NestedValues_nested_entities()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new NestedValues
            {
                Entities = new[] { new TestEntity { Name = "hi" }, new TestEntity { Name = "there" } },
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.CollectionJsonConverterTest+NestedValues"",""values"":null,""uniqueTags"":[],""entities"":[{""$type"":""Kephas.Serialization.Json.Tests.Converters.CollectionJsonConverterTest+TestEntity"",""name"":""hi"",""personalSite"":null},{""$type"":""Kephas.Serialization.Json.Tests.Converters.CollectionJsonConverterTest+TestEntity"",""name"":""there"",""personalSite"":null}],""ages"":null,""names"":null}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_NestedValues_nested_entities()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(
                @"{""$type"":""Kephas.Serialization.Json.Tests.Converters.CollectionJsonConverterTest+NestedValues"",""values"":null,""entities"":[{""$type"":""Kephas.Serialization.Json.Tests.Converters.CollectionJsonConverterTest+TestEntity"",""name"":""hi"",""personalSite"":null},{""$type"":""Kephas.Serialization.Json.Tests.Converters.CollectionJsonConverterTest+TestEntity"",""name"":""there"",""personalSite"":null}]}");

            Assert.IsInstanceOf<NestedValues>(obj);
            var nestedValues = (NestedValues)obj;
            Assert.AreEqual("hi", nestedValues.Entities.First().Name);
            Assert.AreEqual("there", nestedValues.Entities.Skip(1).First().Name);
            Assert.IsNull(nestedValues.Values);
        }

        [Test]
        public async Task SerializeAsync_NestedValues()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new NestedValues
            {
                Ages = new HashSet<int> { 1, 2, 3 },
                Names = new List<string> { "you", "and", "me" },
                Values = new ConcurrentQueue<object>(new object[] { 1, "a" }),
                UniqueTags = { "my", "oh", "my" },
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.CollectionJsonConverterTest+NestedValues"",""values"":[1,""a""],""uniqueTags"":[""my"",""oh""],""entities"":null,""ages"":[1,2,3],""names"":[""you"",""and"",""me""]}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_NestedValues()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(
                @"{""$type"":""Kephas.Serialization.Json.Tests.Converters.CollectionJsonConverterTest+NestedValues"",""values"":[1,""a""],""uniqueTags"":[""my"",""oh"",""my""],""entities"":null,""ages"":[1,2,3,2],""names"":[""you"",""and"",""me""]}");

            Assert.IsInstanceOf<NestedValues>(obj);
            var nestedValues = (NestedValues)obj;
            Assert.AreEqual(1, nestedValues.Values.First());
            Assert.AreEqual("a", nestedValues.Values.Skip(1).First());
            Assert.AreEqual(2, nestedValues.UniqueTags.Count);
            Assert.AreEqual("my", nestedValues.UniqueTags.First());
            Assert.AreEqual("oh", nestedValues.UniqueTags.Skip(1).First());
            Assert.IsNull(nestedValues.Entities);
            Assert.AreEqual(3, nestedValues.Ages.Count);
            Assert.AreEqual(1, nestedValues.Ages.First());
            Assert.AreEqual(2, nestedValues.Ages.Skip(1).First());
            Assert.AreEqual(3, nestedValues.Ages.Skip(2).First());
            Assert.AreEqual(3, nestedValues.Names.Count);
            Assert.AreEqual("you", nestedValues.Names[0]);
            Assert.AreEqual("and", nestedValues.Names[1]);
            Assert.AreEqual("me", nestedValues.Names[2]);
        }

        public class TestEntity
        {
            public string Name { get; set; }

            public Uri PersonalSite { get; set; }
        }

        public class NestedValues
        {
            public IReadOnlyCollection<object> Values { get; set; }

            public ICollection<string> UniqueTags { get; } = new HashSet<string>();

            public ICollection<TestEntity> Entities { get; set; }

            public HashSet<int> Ages { get; set; }

            public IList<string> Names { get; set; }
        }
    }
}