namespace Kephas.Core.Tests.Serialization
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Serialization;
    using Kephas.Serialization.Json;
    using Kephas.Serialization.Xml;

    using NUnit.Framework;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

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
            var serializer = Mock.Create<ISerializer>(Behavior.Strict);
            serializer.Arrange(
                s =>
                s.DeserializeAsync(
                    Arg.Matches<TextReader>(r => r.ReadToEnd() == content),
                    Arg.Matches<ISerializationContext>(c => c != null),
                    Arg.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult((object)new TestEntity { Name = content }));
            return serializer;
        }

        private ISerializer CreateSerializerMock(string result)
        {
            var serializer = Mock.Create<ISerializer>(Behavior.Strict);
            serializer.Arrange(
                s =>
                s.SerializeAsync(
                    Arg.IsAny<object>(),
                    Arg.Matches<TextWriter>(w => w != null),
                    Arg.Matches<ISerializationContext>(c => c != null),
                    Arg.IsAny<CancellationToken>()))
                .DoInstead<object, TextWriter, ISerializationContext, CancellationToken>((o, w, ctx, c) => w.Write(result))
                .Returns<object, TextWriter, ISerializationContext, CancellationToken>(
                    (o, w, ctx, c) => TaskHelper.CompletedTask);
            return serializer;
        }

        private ISerializationService CreateSerializationServiceMock<TFormat>(ISerializer serializer)
        {
            var serializationService = Mock.Create<ISerializationService>(Behavior.Strict);
            var ambientServicesMock = Mock.Create<IAmbientServices>();
            serializationService.Arrange(s => s.AmbientServices).Returns(() => ambientServicesMock);
            serializationService.Arrange(
                s =>
                s.GetSerializer(Arg.Matches<ISerializationContext>(ctx => ctx != null && ctx.FormatType == typeof(TFormat))))
                .Returns(serializer);
            return serializationService;
        }

        public class TestEntity
        {
            public string Name { get; set; }
        }
    }
}