// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializerTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="JsonSerializer" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Reflection;
    using Kephas.Serialization.Json;

    using NSubstitute;

    using NUnit.Framework;

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
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new TestEntity
                          {
                              Name = "John Doe"
                          };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity, Kephas.Serialization.Json.Tests"",""name"":""John Doe""}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_with_serialized_types()
        {
            var settingsProvider = new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity, Kephas.Serialization.Json.Tests"",""name"":""John Doe""}";
            var obj = await serializer.DeserializeAsync(serializedObj);

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
            context.FormatType.Returns(typeof(JsonFormat));
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
            context.FormatType.Returns(typeof(JsonFormat));
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
            var jsonSerializer = serializationService.GetSerializer(SerializationContext.Create<JsonFormat>(serializationService));

            Assert.IsInstanceOf<JsonSerializer>(jsonSerializer);
        }

        public class TestEntity
        {
            public string Name { get; set; }
        }
    }
}