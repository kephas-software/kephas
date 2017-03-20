namespace Kephas.Core.Tests.Serialization
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Serialization;
    using Kephas.Serialization.Json;
    using Kephas.Serialization.Xml;

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
            var serializationService = this.CreateSerializationServiceMock<JsonFormat>(serializer);

            var result = await SerializationExtensions.SerializeAsync<JsonFormat>(serializationService, new TestEntity());
            Assert.AreEqual("ok", result);
        }

        [Test]
        public async Task JsonSerializeAsync_SerializationService()
        {
            var serializer = this.CreateSerializerMock("ok");
            var serializationService = this.CreateSerializationServiceMock<JsonFormat>(serializer);

            var result = await SerializationExtensions.JsonSerializeAsync(serializationService, new TestEntity());
            Assert.AreEqual("ok", result);
        }

        [Test]
        public async Task XmlSerializeAsync_SerializationService()
        {
            var serializer = this.CreateSerializerMock("ok");
            var serializationService = this.CreateSerializationServiceMock<XmlFormat>(serializer);

            var result = await SerializationExtensions.XmlSerializeAsync(serializationService, new TestEntity());
            Assert.AreEqual("ok", result);
        }

        [Test]
        public async Task DeserializeAsync_SerializationService()
        {
            var deserializer = this.CreateDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<JsonFormat>(deserializer);

            var result = await SerializationExtensions.DeserializeAsync<JsonFormat>(serializationService, "my object");
            Assert.IsInstanceOf<TestEntity>(result);
            Assert.AreEqual("my object", (result as TestEntity)?.Name);
        }

        [Test]
        public async Task JsonDeserializeAsync_SerializationService()
        {
            var deserializer = this.CreateDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<JsonFormat>(deserializer);

            var result = await SerializationExtensions.JsonDeserializeAsync(serializationService, "my object");
            Assert.IsInstanceOf<TestEntity>(result);
            Assert.AreEqual("my object", (result as TestEntity)?.Name);
        }

        [Test]
        public async Task XmlDeserializeAsync_SerializationService()
        {
            var deserializer = this.CreateDeserializerMock("my object");
            var serializationService = this.CreateSerializationServiceMock<XmlFormat>(deserializer);

            var result = await SerializationExtensions.XmlDeserializeAsync(serializationService, "my object");
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

        private ISerializationService CreateSerializationServiceMock<TFormat>(ISerializer serializer)
        {
            var serializationService = Substitute.For<ISerializationService>(/*Behavior.Strict*/);
            var ambientServicesMock = Substitute.For<IAmbientServices>();
            serializationService.AmbientServices.Returns(ambientServicesMock);
            serializationService.GetSerializer(Arg.Is<ISerializationContext>(ctx => ctx != null && ctx.FormatType == typeof(TFormat)))
                .Returns(serializer);
            return serializationService;
        }

        public class TestEntity
        {
            public string Name { get; set; }
        }
    }
}