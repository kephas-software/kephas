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
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using NUnit.Framework;

    using Kephas.Serialization.Json;

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
            var serializer = new JsonSerializer();
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
            var serializer = new JsonSerializer();
            var serializedObj = @"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity, Kephas.Serialization.Json.Tests"",""name"":""John Doe""}";
            var obj = await serializer.DeserializeAsync(serializedObj);

            Assert.IsInstanceOf<TestEntity>(obj);

            var testEntity = (TestEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Name);
        }

        [Test]
        public async Task DeserializeAsync_with_provided_type()
        {
            var serializer = new JsonSerializer();
            var serializedObj = @"{""name"":""John Doe""}";
            var obj = await serializer.DeserializeAsync(serializedObj, SerializationContext.Create<JsonFormat>());

            Assert.IsInstanceOf<TestEntity>(obj);

            var testEntity = (TestEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Name);
        }

        [Test]
        public async Task Composition()
        {
            var asBuilder = new AmbientServicesBuilder();
            await asBuilder.WithMefCompositionContainerAsync(
                b =>
                b.WithAssemblies(new[] { typeof(ISerializationService).Assembly, typeof(JsonSerializer).Assembly }));
            var ambientServices = asBuilder.AmbientServices;
            var serializationService = ambientServices.CompositionContainer.GetExport<ISerializationService>();
            var jsonSerializer = serializationService.GetSerializer(SerializationContext.Create<JsonFormat>());

            Assert.IsInstanceOf<JsonSerializer>(jsonSerializer);
        }

        public class TestEntity
        {
            public string Name { get; set; }
        }
    }
}