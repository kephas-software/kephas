// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Tests for <see cref="JsonSerializer" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Dynamic;
    using Kephas.Injection;
    using Kephas.Logging;
    using Kephas.Net.Mime;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Serialization.Json;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="JsonSerializer"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class JsonSerializerTest : SerializationTestBase
    {
        [Test]
        public async Task SerializeAsync_injection()
        {
            var container = this.BuildServiceProvider();
            var serializationService = container.Resolve<ISerializationService>();

            var obj = new TestEntity
            {
                Name = "John Doe",
                PersonalSite = new Uri("http://site.com/my-site"),
            };

            var serializedObj = await serializationService.JsonSerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity"",""name"":""John Doe"",""personalSite"":""http://site.com/my-site""}", serializedObj);
        }

        [Test]
        public async Task SerializeAsync()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider() ;
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new TestEntity
            {
                Name = "John Doe",
                PersonalSite = new Uri("http://site.com/my-site"),
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity"",""name"":""John Doe"",""personalSite"":""http://site.com/my-site""}", serializedObj);
        }

        [Test]
        public void Serialize()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new TestEntity
                          {
                              Name = "John Doe",
                              PersonalSite = new Uri("http://site.com/my-site"),
                          };
            var serializedObj = serializer.Serialize(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity"",""name"":""John Doe"",""personalSite"":""http://site.com/my-site""}", serializedObj);
        }

        [Test]
        public async Task SerializeAsync_indented()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new TestEntity
            {
                Name = "John Doe",
                PersonalSite = new Uri("http://site.com/my-site"),
            };
            var serializationContext = new SerializationContext(Substitute.For<IServiceProvider>(), Substitute.For<ISerializationService>(), typeof(JsonMediaType)) { Indent = true };
            var serializedObj = await serializer.SerializeAsync(obj, serializationContext);

            Assert.AreEqual(
                "{\r\n  \"$type\": \"Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity\",\r\n  \"name\": \"John Doe\",\r\n  \"personalSite\": \"http://site.com/my-site\"\r\n}"
                    .Replace("\r\n", Environment.NewLine),
                serializedObj);
        }

        [Test]
        public void Serialize_indented()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new TestEntity
                          {
                              Name = "John Doe",
                              PersonalSite = new Uri("http://site.com/my-site"),
                          };
            var serializationContext = new SerializationContext(Substitute.For<IServiceProvider>(), Substitute.For<ISerializationService>(), typeof(JsonMediaType)) { Indent = true };
            var serializedObj = serializer.Serialize(obj, serializationContext);

            Assert.AreEqual(
                "{\r\n  \"$type\": \"Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity\",\r\n  \"name\": \"John Doe\",\r\n  \"personalSite\": \"http://site.com/my-site\"\r\n}"
                    .Replace("\r\n", Environment.NewLine),
                serializedObj);
        }

        [Test]
        public async Task SerializeAsync_with_type_info()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new TestEntity
            {
                Name = "John Doe",
                PersonalSite = new Uri("http://site.com/my-site"),
            };
            var serializationContext = new SerializationContext(Substitute.For<IServiceProvider>(), Substitute.For<ISerializationService>(), typeof(JsonMediaType)) { IncludeTypeInfo = true };
            var serializedObj = await serializer.SerializeAsync(obj, serializationContext);

            Assert.AreEqual(
                "{\"$type\":\"Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity\",\"name\":\"John Doe\",\"personalSite\":\"http://site.com/my-site\"}",
                serializedObj);
        }

        [Test]
        public async Task SerializeAsync_without_null_values()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new TestEntity
            {
                Name = "John Doe",
                PersonalSite = null,
            };
            var serializationContext = new SerializationContext(Substitute.For<IServiceProvider>(), Substitute.For<ISerializationService>(), typeof(JsonMediaType)) { IncludeNullValues = false };
            var serializedObj = await serializer.SerializeAsync(obj, serializationContext);

            Assert.AreEqual(
                "{\"$type\":\"Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity\",\"name\":\"John Doe\"}",
                serializedObj);
        }

        [Test]
        public async Task SerializeAsync_with_null_values()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new TestEntity
            {
                Name = "John Doe",
                PersonalSite = null,
            };
            var serializationContext = new SerializationContext(Substitute.For<IServiceProvider>(), Substitute.For<ISerializationService>(), typeof(JsonMediaType)) { IncludeNullValues = true };
            var serializedObj = await serializer.SerializeAsync(obj, serializationContext);

            Assert.AreEqual(
                "{\"$type\":\"Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity\",\"name\":\"John Doe\",\"personalSite\":null}",
                serializedObj);
        }

        [Test]
        public void Serialize_with_type_info()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new TestEntity
            {
                Name = "John Doe",
                PersonalSite = new Uri("http://site.com/my-site"),
            };
            var serializationContext = new SerializationContext(Substitute.For<IServiceProvider>(), Substitute.For<ISerializationService>(), typeof(JsonMediaType)) { IncludeTypeInfo = true };
            var serializedObj = serializer.Serialize(obj, serializationContext);

            Assert.AreEqual(
                "{\"$type\":\"Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity\",\"name\":\"John Doe\",\"personalSite\":\"http://site.com/my-site\"}",
                serializedObj);
        }

        [Test]
        public async Task SerializeAsync_no_type_info()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new TestEntity
            {
                Name = "John Doe",
                PersonalSite = new Uri("http://site.com/my-site"),
            };
            var serializationContext = new SerializationContext(Substitute.For<IServiceProvider>(), Substitute.For<ISerializationService>(), typeof(JsonMediaType)) { IncludeTypeInfo = false };
            var serializedObj = await serializer.SerializeAsync(obj, serializationContext);

            Assert.AreEqual(
                "{\"name\":\"John Doe\",\"personalSite\":\"http://site.com/my-site\"}",
                serializedObj);
        }

        [Test]
        public void Serialize_no_type_info()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new TestEntity
            {
                Name = "John Doe",
                PersonalSite = new Uri("http://site.com/my-site"),
            };
            var serializationContext = new SerializationContext(Substitute.For<IServiceProvider>(), Substitute.For<ISerializationService>(), typeof(JsonMediaType)) { IncludeTypeInfo = false };
            var serializedObj = serializer.Serialize(obj, serializationContext);

            Assert.AreEqual(
                "{\"name\":\"John Doe\",\"personalSite\":\"http://site.com/my-site\"}",
                serializedObj);
        }

        [Test]
        public async Task SerializeAsync_with_type_property()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new TestWithType
            {
                Type = typeof(string),
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+TestWithType"",""type"":""System.String""}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_with_type_property()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);

            var serializedObj = @"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+TestWithType"",""type"":""System.String""}";
            var obj = (TestWithType)await serializer.DeserializeAsync(serializedObj);

            Assert.AreEqual(typeof(string), obj.Type);
        }

        [Test]
        public async Task DeserializeAsync_untyped()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""hi"":""there"",""my"":""friend""}";
            var obj = await serializer.DeserializeAsync(serializedObj);

            Assert.IsInstanceOf<IDictionary<string, object>>(obj);

            var dict = (IDictionary<string, object>)obj;
            Assert.AreEqual("there", dict["hi"]);
            Assert.AreEqual("friend", dict["my"]);
        }

        [Test]
        public void Deserialize_untyped()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""hi"":""there"",""my"":""friend""}";
            var obj = serializer.Deserialize(serializedObj);

            Assert.IsInstanceOf<IDictionary<string, object>>(obj);

            var dict = (IDictionary<string, object>)obj;
            Assert.AreEqual("there", dict["hi"]);
            Assert.AreEqual("friend", dict["my"]);
        }

        [Test]
        public async Task DeserializeAsync_dictionary()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""hi"":""there"",""my"":""friend""}";
            var obj = await serializer.DeserializeAsync(serializedObj, new SerializationContext(Substitute.For<IServiceProvider>(), Substitute.For<ISerializationService>(), typeof(JsonMediaType)) { RootObjectType = typeof(IDictionary<string, object>) });

            Assert.IsInstanceOf<IDictionary<string, object>>(obj);

            var dict = (IDictionary<string, object>)obj;
            Assert.AreEqual("there", dict["hi"]);
            Assert.AreEqual("friend", dict["my"]);
        }

        [Test]
        public void Deserialize_dictionary()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""hi"":""there"",""my"":""friend""}";
            var obj = serializer.Deserialize(serializedObj, new SerializationContext(Substitute.For<IServiceProvider>(), Substitute.For<ISerializationService>(), typeof(JsonMediaType)) { RootObjectType = typeof(IDictionary<string, object>) });

            Assert.IsInstanceOf<IDictionary<string, object>>(obj);

            var dict = (IDictionary<string, object>)obj;
            Assert.AreEqual("there", dict["hi"]);
            Assert.AreEqual("friend", dict["my"]);
        }

        [Test]
        public async Task DeserializeAsync_with_serialized_types()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity, Kephas.Serialization.NewtonsoftJson.Tests"",""name"":""John Doe"",""personalSite"":""http://site.com/my-site""}";
            var obj = await serializer.DeserializeAsync(serializedObj);

            Assert.IsInstanceOf<TestEntity>(obj);

            var testEntity = (TestEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Name);
            Assert.AreEqual(new Uri("http://site.com/my-site"), testEntity.PersonalSite);
        }

        [Test]
        public void Deserialize_with_serialized_types()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity, Kephas.Serialization.NewtonsoftJson.Tests"",""name"":""John Doe"",""personalSite"":""http://site.com/my-site""}";
            var obj = serializer.Deserialize(serializedObj);

            Assert.IsInstanceOf<TestEntity>(obj);

            var testEntity = (TestEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Name);
            Assert.AreEqual(new Uri("http://site.com/my-site"), testEntity.PersonalSite);
        }

        [Test]
        public async Task DeserializeAsync_with_serialized_types_expando()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+ExpandoEntity, Kephas.Serialization.NewtonsoftJson.Tests"",""description"":""John Doe""}";
            var obj = await serializer.DeserializeAsync(serializedObj);

            Assert.IsInstanceOf<ExpandoEntity>(obj);

            var testEntity = (ExpandoEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Description);
        }

        [Test]
        public async Task DeserializeAsync_with_serialized_types_expando_extended_members()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+ExpandoEntity"",""description"":""John Doe"", ""gigi"": ""belogea""}";
            var obj = await serializer.DeserializeAsync(serializedObj);

            Assert.IsInstanceOf<ExpandoEntity>(obj);

            var testEntity = (ExpandoEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Description);
            Assert.AreEqual("belogea", testEntity["gigi"]);
        }

        [Test]
        public async Task DeserializeAsync_with_in_string_provided_type_no_assembly_name()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity"",""name"":""John Doe""}";
            var context = Substitute.For<ISerializationContext>();
            context.MediaType.Returns(typeof(JsonMediaType));
            context.RootObjectType.Returns(typeof(TestEntity));
            context.RootObjectFactory.Returns((Func<object>)null);
            var obj = await serializer.DeserializeAsync(serializedObj, context);

            Assert.IsInstanceOf<TestEntity>(obj);

            var testEntity = (TestEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Name);
        }

        [Test]
        public async Task DeserializeAsync_with_in_string_provided_type()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity, Kephas.Serialization.NewtonsoftJson.Tests"",""name"":""John Doe""}";
            var context = Substitute.For<ISerializationContext>();
            context.MediaType.Returns(typeof(JsonMediaType));
            context.RootObjectType.Returns(typeof(TestEntity));
            context.RootObjectFactory.Returns((Func<object>)null);
            var obj = await serializer.DeserializeAsync(serializedObj, context);

            Assert.IsInstanceOf<TestEntity>(obj);

            var testEntity = (TestEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Name);
        }

        [Test]
        public async Task DeserializeAsync_with_runtime_provided_type()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""name"":""John Doe""}";
            var context = Substitute.For<ISerializationContext>();
            context.MediaType.Returns(typeof(JsonMediaType));
            context.RootObjectType.Returns(typeof(TestEntity));
            context.RootObjectFactory.Returns((Func<object>)null);
            var obj = await serializer.DeserializeAsync(serializedObj, context);

            Assert.IsInstanceOf<TestEntity>(obj);

            var testEntity = (TestEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Name);
        }

        [Test]
        public async Task JsonSerializer_injection_autofac()
        {
            var ambientServices = this.CreateAmbientServices()
                .WithStaticAppRuntime()
                .BuildWithAutofac(
                    b =>
                    b.WithAssemblies(
                        typeof(IServiceProvider).Assembly,
                        typeof(ISerializationService).Assembly,
                        typeof(JsonSerializer).Assembly,
                        typeof(DefaultTypeResolver).Assembly));
            var serializers = ambientServices.Injector.ResolveMany<Lazy<ISerializer, SerializerMetadata>>();
            var jsonSerializer = serializers.SingleOrDefault(s => s.Metadata.MediaType == typeof(JsonMediaType))?.Value;

            Assert.IsInstanceOf<JsonSerializer>(jsonSerializer);
        }

        [Test]
        public async Task JsonSerializer_injection_lite()
        {
            var ambientServices = this.CreateAmbientServices()
                .WithStaticAppRuntime()
                .BuildWithLite(
                    b =>
                        b.WithAssemblies(
                            typeof(IServiceProvider).Assembly,
                            typeof(ISerializationService).Assembly,
                            typeof(JsonSerializer).Assembly,
                            typeof(DefaultTypeResolver).Assembly));
            var serializers = ambientServices.Injector.ResolveMany<Lazy<ISerializer, SerializerMetadata>>();
            var jsonSerializer = serializers.SingleOrDefault(s => s.Metadata.MediaType == typeof(JsonMediaType))?.Value;

            Assert.IsInstanceOf<JsonSerializer>(jsonSerializer);
        }

        [Test]
        public async Task SerializeAsync_datacontract_hierarchy()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new Node
            {
                Name = "root",
                Children = new List<Node>
                {
                    new Node { Name = "Left" },
                    new Node { Name = "Right" },
                },
            };

            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+Node"",""name"":""root"",""children"":[{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+Node"",""name"":""Left"",""children"":null},{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+Node"",""name"":""Right"",""children"":null}]}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_datacontract_hierarcy()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+Node"",""name"":""root"",""children"":[{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+Node"",""name"":""Left""},{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+Node"",""name"":""Right""}]}";
            var obj = await serializer.DeserializeAsync(serializedObj);

            Assert.IsInstanceOf<Node>(obj);

            var node = (Node)obj;
            Assert.AreEqual("root", node.Name);
            Assert.AreEqual(2, node.Children.Count);
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

        [DataContract]
        public class Node
        {
            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public ICollection<Node> Children { get; set; }

            public string Path { get; set; }
        }
    }
}