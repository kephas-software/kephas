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
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Dynamic;
    using Kephas.Net.Mime;
    using Kephas.Reflection;
    using Kephas.Serialization.Json;

    using Newtonsoft.Json.Linq;

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
        public async Task SerializeAsync_Composition()
        {
            var container = this.CreateContainer();
            var serializationService = container.GetExport<ISerializationService>();

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
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(new DefaultAssemblyLoader()));
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
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(new DefaultAssemblyLoader()));
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
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new TestEntity
            {
                Name = "John Doe",
                PersonalSite = new Uri("http://site.com/my-site"),
            };
            var serializationContext = new SerializationContext(Substitute.For<ICompositionContext>(), Substitute.For<ISerializationService>(), typeof(JsonMediaType)) { Indent = true };
            var serializedObj = await serializer.SerializeAsync(obj, serializationContext);

            Assert.AreEqual(
                "{\r\n  \"$type\": \"Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity\",\r\n  \"name\": \"John Doe\",\r\n  \"personalSite\": \"http://site.com/my-site\"\r\n}"
                    .Replace("\r\n", Environment.NewLine),
                serializedObj);
        }

        [Test]
        public void Serialize_indented()
        {
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new TestEntity
                          {
                              Name = "John Doe",
                              PersonalSite = new Uri("http://site.com/my-site"),
                          };
            var serializationContext = new SerializationContext(Substitute.For<ICompositionContext>(), Substitute.For<ISerializationService>(), typeof(JsonMediaType)) { Indent = true };
            var serializedObj = serializer.Serialize(obj, serializationContext);

            Assert.AreEqual(
                "{\r\n  \"$type\": \"Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity\",\r\n  \"name\": \"John Doe\",\r\n  \"personalSite\": \"http://site.com/my-site\"\r\n}"
                    .Replace("\r\n", Environment.NewLine),
                serializedObj);
        }

        [Test]
        public async Task SerializeAsync_Expando()
        {
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new ExpandoEntity
            {
                Description = "John Doe"
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+ExpandoEntity"",""description"":""John Doe""}", serializedObj);
        }

        [Test, Ignore("The solution of deserializing objects by default in dictionaries still pending... Try wrapping the resulted IDictionary<string, JToken> into a IDictionary<string, object>.")]
        public async Task DeserializeAsync_untyped()
        {
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""hi"":""there"",""my"":""friend""}";
            var obj = await serializer.DeserializeAsync(serializedObj);

            Assert.IsInstanceOf<IDictionary<string, object>>(obj);

            var dict = (IDictionary<string, object>)obj;
            Assert.AreEqual("there", dict["hi"]);
            Assert.AreEqual("friend", dict["my"]);
        }

        [Test]
        public async Task DeserializeAsync_dictionary()
        {
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""hi"":""there"",""my"":""friend""}";
            var obj = await serializer.DeserializeAsync(serializedObj, new SerializationContext(Substitute.For<ICompositionContext>(), Substitute.For<ISerializationService>(), typeof(JsonMediaType)) { RootObjectType = typeof(IDictionary<string, object>) });

            Assert.IsInstanceOf<IDictionary<string, object>>(obj);

            var dict = (IDictionary<string, object>)obj;
            Assert.AreEqual("there", dict["hi"]);
            Assert.AreEqual("friend", dict["my"]);
        }

        [Test]
        public void Deserialize_dictionary()
        {
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""hi"":""there"",""my"":""friend""}";
            var obj = serializer.Deserialize(serializedObj, new SerializationContext(Substitute.For<ICompositionContext>(), Substitute.For<ISerializationService>(), typeof(JsonMediaType)) { RootObjectType = typeof(IDictionary<string, object>) });

            Assert.IsInstanceOf<IDictionary<string, object>>(obj);

            var dict = (IDictionary<string, object>)obj;
            Assert.AreEqual("there", dict["hi"]);
            Assert.AreEqual("friend", dict["my"]);
        }

        [Test]
        public async Task DeserializeAsync_with_serialized_types()
        {
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity, Kephas.Serialization.Json.Tests"",""name"":""John Doe"",""personalSite"":""http://site.com/my-site""}";
            var obj = await serializer.DeserializeAsync(serializedObj);

            Assert.IsInstanceOf<TestEntity>(obj);

            var testEntity = (TestEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Name);
            Assert.AreEqual(new Uri("http://site.com/my-site"), testEntity.PersonalSite);
        }

        [Test]
        public void Deserialize_with_serialized_types()
        {
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity, Kephas.Serialization.Json.Tests"",""name"":""John Doe"",""personalSite"":""http://site.com/my-site""}";
            var obj = serializer.Deserialize(serializedObj);

            Assert.IsInstanceOf<TestEntity>(obj);

            var testEntity = (TestEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Name);
            Assert.AreEqual(new Uri("http://site.com/my-site"), testEntity.PersonalSite);
        }

        [Test]
        public async Task DeserializeAsync_with_serialized_types_expando()
        {
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+ExpandoEntity, Kephas.Serialization.Json.Tests"",""description"":""John Doe""}";
            var obj = await serializer.DeserializeAsync(serializedObj);

            Assert.IsInstanceOf<ExpandoEntity>(obj);

            var testEntity = (ExpandoEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Description);
        }

        [Test]
        public async Task DeserializeAsync_with_in_string_provided_type_no_assembly_name()
        {
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(new DefaultAssemblyLoader()));
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
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity, Kephas.Serialization.Json.Tests"",""name"":""John Doe""}";
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
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(new DefaultAssemblyLoader()));
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
        public async Task JsonSerializer_Composition()
        {
            var ambientServices = new AmbientServices().WithMefCompositionContainer(
                b =>
                b.WithAssemblies(new[] { typeof(ISerializationService).GetTypeInfo().Assembly, typeof(JsonSerializer).GetTypeInfo().Assembly }));
            var serializationService = ambientServices.CompositionContainer.GetExport<ISerializationService>();
            var jsonSerializer = serializationService.GetSerializer(SerializationContext.Create<JsonMediaType>(serializationService));

            Assert.IsInstanceOf<JsonSerializer>(jsonSerializer);
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
    }
}