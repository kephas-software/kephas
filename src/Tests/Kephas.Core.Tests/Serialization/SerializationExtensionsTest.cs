namespace Kephas.Core.Tests.Serialization
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Net.Mime;
    using Kephas.Serialization;
    using Kephas.Testing;
    using NSubstitute;

    using NUnit.Framework;

    using TaskHelper = Kephas.Threading.Tasks.TaskHelper;

    [TestFixture]
    public class SerializationExtensionsTest : TestBase
    {
        [Test]
        public async Task SerializeAsync_SerializationService()
        {
            var serializer = this.CreateStringSerializerMock("ok");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(serializer);

            var result = await serializationService.SerializeAsync<JsonMediaType>(new TestEntity());
            Assert.AreEqual("ok", result);
        }

        [Test]
        public void Serialize_SerializationService()
        {
            var serializer = this.CreateStringSerializerMock("ok");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(serializer);

            var result = serializationService.Serialize<JsonMediaType>(new TestEntity());
            Assert.AreEqual("ok", result);
        }

        [Test]
        public async Task JsonSerializeAsync_SerializationService()
        {
            var serializer = this.CreateStringSerializerMock("ok");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(serializer);

            var result = await serializationService.JsonSerializeAsync(new TestEntity());
            Assert.AreEqual("ok", result);
        }

        [Test]
        public void JsonSerialize_SerializationService()
        {
            var serializer = this.CreateStringSerializerMock("ok");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(serializer);

            var result = serializationService.JsonSerialize(new TestEntity());
            Assert.AreEqual("ok", result);
        }

        [Test]
        public async Task XmlSerializeAsync_SerializationService()
        {
            var serializer = this.CreateStringSerializerMock("ok");
            var serializationService = this.CreateSerializationServiceMock<XmlMediaType>(serializer);

            var result = await SerializationExtensions.XmlSerializeAsync(serializationService, new TestEntity());
            Assert.AreEqual("ok", result);
        }

        [Test]
        public void XmlSerialize_SerializationService()
        {
            var serializer = this.CreateStringSerializerMock("ok");
            var serializationService = this.CreateSerializationServiceMock<XmlMediaType>(serializer);

            var result = SerializationExtensions.XmlSerialize(serializationService, new TestEntity());
            Assert.AreEqual("ok", result);
        }

        [Test]
        public async Task DeserializeAsync_SerializationService()
        {
            var deserializer = this.CreateStringDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(deserializer);

            var result = await serializationService.DeserializeAsync<JsonMediaType>("my object");
            Assert.IsInstanceOf<TestEntity>(result);
            Assert.AreEqual("my object", (result as TestEntity)?.Name);
        }

        [Test]
        public void DeserializeAsync_SerializationService_null()
        {
            var deserializer = this.CreateStringDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(deserializer);

            Assert.ThrowsAsync<ArgumentNullException>(() => serializationService.DeserializeAsync<JsonMediaType>(null));
        }

        [Test]
        public void Deserialize_SerializationService()
        {
            var deserializer = this.CreateStringSyncDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(deserializer);

            var result = serializationService.Deserialize<JsonMediaType>("my object");
            Assert.IsInstanceOf<TestEntity>(result);
            Assert.AreEqual("my object", (result as TestEntity)?.Name);
        }

        [Test]
        public void Deserialize_SerializationService_null()
        {
            var deserializer = this.CreateStringSyncDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(deserializer);

            Assert.Throws<ArgumentNullException>(() => serializationService.Deserialize<JsonMediaType>(null));
        }

        [Test]
        public async Task JsonDeserializeAsync_SerializationService()
        {
            var deserializer = this.CreateStringDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(deserializer);

            var result = await serializationService.JsonDeserializeAsync("my object");
            Assert.IsInstanceOf<TestEntity>(result);
            Assert.AreEqual("my object", (result as TestEntity)?.Name);
        }

        [Test]
        public void JsonDeserialize_SerializationService()
        {
            var deserializer = this.CreateStringSyncDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(deserializer);

            var result = serializationService.JsonDeserialize("my object");
            Assert.IsInstanceOf<TestEntity>(result);
            Assert.AreEqual("my object", (result as TestEntity)?.Name);
        }

        [Test]
        public async Task XmlDeserializeAsync_SerializationService()
        {
            var deserializer = this.CreateStringDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<XmlMediaType>(deserializer);

            var result = await SerializationExtensions.XmlDeserializeAsync(serializationService, "my object");
            Assert.IsInstanceOf<TestEntity>(result);
            Assert.AreEqual("my object", (result as TestEntity)?.Name);
        }

        [Test]
        public void XmlDeserialize_SerializationService()
        {
            var deserializer = this.CreateStringSyncDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<XmlMediaType>(deserializer);

            var result = SerializationExtensions.XmlDeserialize(serializationService, "my object");
            Assert.IsInstanceOf<TestEntity>(result);
            Assert.AreEqual("my object", (result as TestEntity)?.Name);
        }

        private ISerializer CreateStringDeserializerMock(string content)
        {
            var serializer = Substitute.For<ISerializer>(/*Behavior.Strict*/);
            serializer.DeserializeAsync(
                    Arg.Any<string>(),
                    Arg.Is<ISerializationContext>(c => c != null),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((object)new TestEntity { Name = content }));
            return serializer;
        }

#if NETCOREAPP3_1_OR_GREATER
        private ISerializer CreateStringSyncDeserializerMock(string content)
        {
            var serializer = Substitute.For<ISerializer>(/*Behavior.Strict*/);
            serializer.Deserialize(
                    Arg.Any<string>(),
                    Arg.Is<ISerializationContext>(c => c != null))
                .Returns(ci => new TestEntity { Name = content });
            return serializer;
        }
#else
        private ISerializer CreateStringSyncDeserializerMock(string content)
        {
            var serializer = Substitute.For<ISerializer, ISyncSerializer>(/*Behavior.Strict*/);
            ((ISyncSerializer)serializer).Deserialize(
                    Arg.Any<string>(),
                    Arg.Is<ISerializationContext>(c => c != null))
                .Returns(ci => new TestEntity { Name = content });
            return serializer;
        }
#endif

        private ISerializer CreateStringSerializerMock(string result)
        {
            var serializer = Substitute.For<ISerializer>(/*Behavior.Strict*/);
            serializer.SerializeAsync(
                    Arg.Any<object>(),
                    Arg.Is<ISerializationContext>(c => c != null),
                    Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult(result));
#if NETCOREAPP3_1_OR_GREATER
            serializer.Serialize(
                    Arg.Any<object>(),
                    Arg.Is<ISerializationContext>(c => c != null))
                .Returns(ci => result);
#endif
            return serializer;
        }

        public class TestEntity
        {
            public string Name { get; set; }
        }
    }
}