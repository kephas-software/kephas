// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayJsonConverterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class ArrayJsonConverterTest
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
            var obj = await serializer.DeserializeAsync(@"[""one"",2,null]");

            Assert.IsInstanceOf<JObjectList>(obj);
            var list = (JObjectList)obj;
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

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ArrayJsonConverterTest+NestedValues"",""values"":[""one"",2,null]}", serializedObj);
        }

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
        public async Task SerializeAsync_HashSet_nested_object()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new HashSet<object?> { "one", new TestEntity { Name = "gigi" } };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"[""one"",{""$type"":""Kephas.Serialization.Json.Tests.Converters.ArrayJsonConverterTest+TestEntity"",""name"":""gigi"",""personalSite"":null}]", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_HashSet_nested_object()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);

            var obj = await serializer.DeserializeAsync(@"[""one"",{""$type"":""Kephas.Serialization.Json.Tests.Converters.ArrayJsonConverterTest+TestEntity"",""name"":""gigi"",""personalSite"":null}]");

            Assert.IsInstanceOf<JObjectList>(obj);
            var list = (JObjectList)obj;
            Assert.AreEqual("one", list[0]);
            Assert.IsInstanceOf<TestEntity>(list[1]);
        }

        private static DefaultJsonSerializerSettingsProvider GetJsonSerializerSettingsProvider()
        {
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(
                new DefaultTypeResolver(() => AppDomain.CurrentDomain.GetAssemblies()), new RuntimeTypeRegistry(), Substitute.For<ILogManager>());
            return settingsProvider;
        }

        public class TestEntity
        {
            public string Name { get; set; }

            public Uri PersonalSite { get; set; }
        }

        public class ExpandoEntity : Expando
        {
            public string Description { get; set; }
        }

        public class TestWithType
        {
            public Type Type { get; set; }
        }

        public class NestedValues
        {
            public object Values { get; set; }
        }
    }
}