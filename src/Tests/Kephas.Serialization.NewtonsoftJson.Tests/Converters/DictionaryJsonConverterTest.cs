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

    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DictionaryJsonConverterTest : SerializationTestBase
    {

        [Test]
        public async Task SerializeAsync_Dictionary()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new Dictionary<string, object>
            {
                ["Description"] = "John Doe",
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""description"":""John Doe""}", serializedObj);
        }

        [Test]
        public async Task SerializeAsync_Dictionary_nested()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new NestedValues
            {
                Values = new Dictionary<string, object>
                {
                    ["Description"] = "John Doe",
                },
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+NestedValues"",""values"":{""description"":""John Doe""}}", serializedObj);
        }

        [Test]
        public async Task SerializeAsync_Dictionary_nested_object_item()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new NestedValues
            {
                Values = new Dictionary<string, object>
                {
                    ["Item"] = new JsonSerializerTest.TestEntity { Name = "gigi" },
                },
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.DictionaryJsonConverterTest+NestedValues"",""values"":{""item"":{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity"",""name"":""gigi"",""personalSite"":null}}}", serializedObj);
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