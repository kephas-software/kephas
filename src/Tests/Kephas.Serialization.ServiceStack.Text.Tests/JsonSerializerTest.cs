// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializerTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the JSON serializer test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Net.Mime;
    using Kephas.Reflection;

    using NSubstitute;

    using NUnit.Framework;

    using JsonSerializer = Kephas.Serialization.ServiceStack.Text.JsonSerializer;

    /// <summary>
    /// Tests for <see cref="JsonSerializer"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class JsonSerializerTest
    {
        [Test]
        public async Task SerializeAsync()
        {
            var settingsProvider = new DefaultJsonSerializerConfigurator(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new TestEntity
            {
                Name = "John Doe",
                PersonalSite = new Uri("http://site.com/my-site")
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.ServiceStack.Text.Tests.JsonSerializerTest+TestEntity, Kephas.Serialization.ServiceStack.Text.Tests"",""name"":""John Doe"",""personalSite"":""http://site.com/my-site""}", serializedObj);
        }

        [Test]
        public async Task SerializeAsync_Expando()
        {
            var settingsProvider = new DefaultJsonSerializerConfigurator(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new ExpandoEntity
            {
                Description = "John Doe"
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.ServiceStack.Text.Tests.JsonSerializerTest+ExpandoEntity, Kephas.Serialization.ServiceStack.Text.Tests"",""description"":""John Doe""}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_untyped()
        {
            var settingsProvider = new DefaultJsonSerializerConfigurator(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""hi"":""there"",""my"":""friend""}";
            var obj = await serializer.DeserializeAsync(serializedObj);

            Assert.IsInstanceOf<IExpando>(obj);

            var dict = (IExpando)obj;
            Assert.AreEqual("there", dict["hi"]);
            Assert.AreEqual("friend", dict["my"]);
        }

        [Test, Ignore("TODO Must make the Expando implement IDictionary first.")]
        public async Task DeserializeAsync_untyped_dictionary()
        {
            var settingsProvider = new DefaultJsonSerializerConfigurator(new DefaultTypeResolver(new DefaultAssemblyLoader()));
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
            var settingsProvider = new DefaultJsonSerializerConfigurator(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""hi"":""there"",""my"":""friend""}";
            var obj = await serializer.DeserializeAsync(serializedObj, new SerializationContext(Substitute.For<ISerializationService>(), typeof(JsonMediaType)) { RootObjectType = typeof(IDictionary<string, object>) });

            Assert.IsInstanceOf<IDictionary<string, object>>(obj);

            var dict = (IDictionary<string, object>)obj;
            Assert.AreEqual("there", dict["hi"]);
            Assert.AreEqual("friend", dict["my"]);
        }

        [Test]
        public async Task DeserializeAsync_with_serialized_types()
        {
            var settingsProvider = new DefaultJsonSerializerConfigurator(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.ServiceStack.Text.Tests.JsonSerializerTest+TestEntity, Kephas.Serialization.ServiceStack.Text.Tests"",""name"":""John Doe"",""personalSite"":""http://site.com/my-site""}";
            var obj = await serializer.DeserializeAsync(serializedObj);

            Assert.IsInstanceOf<TestEntity>(obj);

            var testEntity = (TestEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Name);
            Assert.AreEqual(new Uri("http://site.com/my-site"), testEntity.PersonalSite);
        }

        [Test, Ignore("The deserialization of custom dynamics is still pending...")]
        public async Task DeserializeAsync_with_serialized_types_expando()
        {
            var settingsProvider = new DefaultJsonSerializerConfigurator(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.ServiceStack.Text.Tests.JsonSerializerTest+ExpandoEntity, Kephas.Serialization.ServiceStack.Text.Tests"",""description"":""John Doe""}";
            var obj = await serializer.DeserializeAsync(serializedObj);

            Assert.IsInstanceOf<ExpandoEntity>(obj);

            var testEntity = (ExpandoEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Description);
        }

        [Test]
        public async Task DeserializeAsync_with_in_string_provided_type()
        {
            var settingsProvider = new DefaultJsonSerializerConfigurator(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.ServiceStack.Text.Tests.JsonSerializerTest+TestEntity, Kephas.Serialization.ServiceStack.Text.Tests"",""name"":""John Doe""}";
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
            var settingsProvider = new DefaultJsonSerializerConfigurator(new DefaultTypeResolver(new DefaultAssemblyLoader()));
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
            var asBuilder = new AmbientServicesBuilder();
            await asBuilder.WithMefCompositionContainerAsync(
                b =>
                b.WithAssemblies(new[] { typeof(ISerializationService).GetTypeInfo().Assembly, typeof(JsonSerializer).GetTypeInfo().Assembly }));
            var ambientServices = asBuilder.AmbientServices;
            var serializationService = ambientServices.CompositionContainer.GetExport<ISerializationService>();
            var jsonSerializer = serializationService.GetSerializer(SerializationContext.Create<JsonMediaType>(serializationService));

            Assert.IsInstanceOf<JsonSerializer>(jsonSerializer);
        }

        [Test]
        public async Task DeserializeAsync_expando_property()
        {
            var settingsProvider = new DefaultJsonSerializerConfigurator(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);

            using (var sr = new StringReader(@"{ ""dynContent"": { ""hi"": ""there"" } }"))
            {
                var serializationService = Substitute.For<ISerializationService>();
                var obj = (TestContext)await serializer.DeserializeAsync(sr, new SerializationContext(serializationService, typeof(JsonMediaType)) { RootObjectType = typeof(TestContext) });

                Assert.AreEqual("there", obj.DynContent["hi"]);
            }
        }

        public class TestContext
        {
            public IExpando DynContent { get; set; }
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