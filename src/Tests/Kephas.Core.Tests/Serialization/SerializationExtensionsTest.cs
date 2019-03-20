namespace Kephas.Core.Tests.Serialization
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Net.Mime;
    using Kephas.Serialization;

    using NSubstitute;

    using NUnit.Framework;

    using TaskHelper = Kephas.Threading.Tasks.TaskHelper;

    [TestFixture]
    public class SerializationExtensionsTest
    {
        [Test]
        public async Task SerializeAsync_SerializationService()
        {
            var serializer = this.CreateSerializerMock("ok");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(serializer);

            var result = await SerializationExtensions.SerializeAsync<JsonMediaType>(serializationService, new TestEntity());
            Assert.AreEqual("ok", result);
        }

        [Test]
        public void Serialize_SerializationService()
        {
            var serializer = this.CreateSerializerMock("ok");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(serializer);

            var result = SerializationExtensions.Serialize<JsonMediaType>(serializationService, new TestEntity());
            Assert.AreEqual("ok", result);
        }

        [Test]
        public async Task JsonSerializeAsync_SerializationService()
        {
            var serializer = this.CreateSerializerMock("ok");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(serializer);

            var result = await SerializationExtensions.JsonSerializeAsync(serializationService, new TestEntity());
            Assert.AreEqual("ok", result);
        }

        [Test]
        public void JsonSerialize_SerializationService()
        {
            var serializer = this.CreateSerializerMock("ok");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(serializer);

            var result = SerializationExtensions.JsonSerialize(serializationService, new TestEntity());
            Assert.AreEqual("ok", result);
        }

        [Test]
        public async Task XmlSerializeAsync_SerializationService()
        {
            var serializer = this.CreateSerializerMock("ok");
            var serializationService = this.CreateSerializationServiceMock<XmlMediaType>(serializer);

            var result = await SerializationExtensions.XmlSerializeAsync(serializationService, new TestEntity());
            Assert.AreEqual("ok", result);
        }

        [Test]
        public void XmlSerialize_SerializationService()
        {
            var serializer = this.CreateSerializerMock("ok");
            var serializationService = this.CreateSerializationServiceMock<XmlMediaType>(serializer);

            var result = SerializationExtensions.XmlSerialize(serializationService, new TestEntity());
            Assert.AreEqual("ok", result);
        }

        [Test]
        public async Task DeserializeAsync_SerializationService()
        {
            var deserializer = this.CreateDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(deserializer);

            var result = await SerializationExtensions.DeserializeAsync<JsonMediaType>(serializationService, "my object");
            Assert.IsInstanceOf<TestEntity>(result);
            Assert.AreEqual("my object", (result as TestEntity)?.Name);
        }

        [Test]
        public async Task DeserializeAsync_SerializationService_null()
        {
            var deserializer = this.CreateDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(deserializer);

            var result = await SerializationExtensions.DeserializeAsync<JsonMediaType>(serializationService, null);
            Assert.IsNull(result);
        }

        [Test]
        public void Deserialize_SerializationService()
        {
            var deserializer = this.CreateDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(deserializer);

            var result = SerializationExtensions.Deserialize<JsonMediaType>(serializationService, "my object");
            Assert.IsInstanceOf<TestEntity>(result);
            Assert.AreEqual("my object", (result as TestEntity)?.Name);
        }

        [Test]
        public void Deserialize_SerializationService_null()
        {
            var deserializer = this.CreateDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(deserializer);

            var result = SerializationExtensions.Deserialize<JsonMediaType>(serializationService, null);
            Assert.IsNull(result);
        }

        [Test]
        public async Task JsonDeserializeAsync_SerializationService()
        {
            var deserializer = this.CreateDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(deserializer);

            var result = await SerializationExtensions.JsonDeserializeAsync(serializationService, "my object");
            Assert.IsInstanceOf<TestEntity>(result);
            Assert.AreEqual("my object", (result as TestEntity)?.Name);
        }

        [Test]
        public void JsonDeserialize_SerializationService()
        {
            var deserializer = this.CreateDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(deserializer);

            var result = SerializationExtensions.JsonDeserialize(serializationService, "my object");
            Assert.IsInstanceOf<TestEntity>(result);
            Assert.AreEqual("my object", (result as TestEntity)?.Name);
        }

        [Test]
        public async Task XmlDeserializeAsync_SerializationService()
        {
            var deserializer = this.CreateDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<XmlMediaType>(deserializer);

            var result = await SerializationExtensions.XmlDeserializeAsync(serializationService, "my object");
            Assert.IsInstanceOf<TestEntity>(result);
            Assert.AreEqual("my object", (result as TestEntity)?.Name);
        }

        [Test]
        public void XmlDeserialize_SerializationService()
        {
            var deserializer = this.CreateDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<XmlMediaType>(deserializer);

            var result = SerializationExtensions.XmlDeserialize(serializationService, "my object");
            Assert.IsInstanceOf<TestEntity>(result);
            Assert.AreEqual("my object", (result as TestEntity)?.Name);
        }

        private ISerializer CreateDeserializerMock(string content)
        {
            var serializer = Substitute.For<ISerializer>(/*Behavior.Strict*/);
            serializer.DeserializeAsync(
                    Arg.Any<StringReader>(),
                    Arg.Is<ISerializationContext>(c => c != null),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((object)new TestEntity { Name = content }));
            return serializer;
        }

        private ISerializer CreateSerializerMock(string result)
        {
            var serializer = Substitute.For<ISerializer>(/*Behavior.Strict*/);
            serializer.SerializeAsync(
                    Arg.Any<object>(),
                    Arg.Is<TextWriter>(w => w != null),
                    Arg.Is<ISerializationContext>(c => c != null),
                    Arg.Any<CancellationToken>())
                .Returns(TaskHelper.CompletedTask)
                .AndDoes(ci => { ci.Arg<TextWriter>().Write(result); });
            return serializer;
        }

        private ISerializationService CreateSerializationServiceMock<TMediaType>(ISerializer serializer)
        {
            var serializationService = Substitute.For<ISerializationService, ICompositionContextAware>(/*Behavior.Strict*/);
            var compositionContextMock = Substitute.For<ICompositionContext>();
            ((ICompositionContextAware)serializationService).CompositionContext.Returns(compositionContextMock);
            serializationService.GetSerializer(Arg.Is<ISerializationContext>(ctx => ctx != null && ctx.MediaType == typeof(TMediaType)))
                .Returns(serializer);
            return serializationService;
        }

        public class TestEntity
        {
            public string Name { get; set; }
        }
    }
}