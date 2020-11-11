// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectJsonConverterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests.Converters
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using NUnit.Framework;

    [TestFixture]
    public class ObjectJsonConverterTest : SerializationTestBase
    {
        [Test]
        public async Task SerializeAsync_object()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new object();
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_object()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync("{}");

            Assert.AreEqual(typeof(JObjectDictionary), obj.GetType());
        }

        [Test]
        public async Task SerializeAsync_array()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new object[0];
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"[]", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_array()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync("[]");

            Assert.AreEqual(typeof(List<object>), obj.GetType());
        }

        [Test]
        public async Task SerializeAsync_tester()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new Tester { Name = "john" };
            var serializedObj = await serializer.SerializeAsync(obj, this.GetSerializationContext(typeof(object)));

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ObjectJsonConverterTest+Tester"",""name"":""john""}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_tester()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync("{}", this.GetSerializationContext(typeof(object)));

            Assert.AreEqual(typeof(JObjectDictionary), obj.GetType());
        }

        [Test]
        public async Task SerializeAsync_tester_array()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new[] { new Tester { Name = "john" } };
            var serializedObj = await serializer.SerializeAsync(obj, this.GetSerializationContext(typeof(object)));

            Assert.AreEqual(@"[{""$type"":""Kephas.Serialization.Json.Tests.Converters.ObjectJsonConverterTest+Tester"",""name"":""john""}]", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_tester_array()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(@"[{""$type"":""Kephas.Serialization.Json.Tests.Converters.ObjectJsonConverterTest+Tester"",""name"":""john""}]", this.GetSerializationContext(typeof(object)));

            Assert.AreEqual(typeof(List<object>), obj.GetType());
            var list = (List<object>)obj;
            Assert.IsInstanceOf<Tester>(list[0]);
        }

        [Test]
        public async Task SerializeAsync_wrapper()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new Wrapper { Object = new Tester { Name = "john" } };
            var serializedObj = await serializer.SerializeAsync(obj, this.GetSerializationContext(typeof(object)));

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ObjectJsonConverterTest+Wrapper"",""object"":{""$type"":""Kephas.Serialization.Json.Tests.Converters.ObjectJsonConverterTest+Tester"",""name"":""john""}}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_wrapper()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ObjectJsonConverterTest+Wrapper"",""object"":{""$type"":""Kephas.Serialization.Json.Tests.Converters.ObjectJsonConverterTest+Tester"",""name"":""john""}}", this.GetSerializationContext(typeof(object)));

            Assert.IsInstanceOf<Wrapper>(obj);
            var wrapper = (Wrapper)obj;
            Assert.IsInstanceOf<Tester>(wrapper.Object);
        }

        [Test]
        public async Task SerializeAsync_wrapper_array()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new Wrapper { Object = new[] { new Tester { Name = "john" } } };
            var serializedObj = await serializer.SerializeAsync(obj, this.GetSerializationContext(typeof(object)));

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ObjectJsonConverterTest+Wrapper"",""object"":[{""$type"":""Kephas.Serialization.Json.Tests.Converters.ObjectJsonConverterTest+Tester"",""name"":""john""}]}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_wrapper_array()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ObjectJsonConverterTest+Wrapper"",""object"":[{""$type"":""Kephas.Serialization.Json.Tests.Converters.ObjectJsonConverterTest+Tester"",""name"":""john""}]}", this.GetSerializationContext(typeof(object)));

            Assert.IsInstanceOf<Wrapper>(obj);
            var wrapper = (Wrapper)obj;
            Assert.IsInstanceOf<List<object>>(wrapper.Object);
            var list = (List<object>)wrapper.Object;
            Assert.IsInstanceOf<Tester>(list[0]);
        }

        public class Tester
        {
            public string Name { get; set; }
        }

        public class Wrapper
        {
            public object Object { get; set; }
        }
    }
}