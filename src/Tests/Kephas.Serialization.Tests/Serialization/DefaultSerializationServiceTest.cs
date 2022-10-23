// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultSerializationServiceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Tests for <see cref="DefaultSerializationService" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;
    using Kephas.Net.Mime;
    using Kephas.Serialization;
    using Kephas.Services;
    using Kephas.Testing;
    using NSubstitute;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="DefaultSerializationService"/>
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class DefaultSerializationServiceTest : TestBase
    {
        [Test]
        [TestCase(typeof(XmlMediaType))]
        [TestCase(typeof(JsonMediaType))]
        public void GetSerializer_not_found(Type mediaType)
        {
            var injectableFactory = Substitute.For<IInjectableFactory>();
            var serializationService = new DefaultSerializationService(injectableFactory, new List<IExportFactory<ISerializer, SerializerMetadata>>());
            injectableFactory.Create<SerializationContext>(serializationService)
                .Returns(ci => new SerializationContext(Substitute.For<IServiceProvider>(), serializationService));
            Assert.Throws<KeyNotFoundException>(() => serializationService.Deserialize("123", ctx => ctx.MediaType = mediaType));
        }

        [Test]
        public void GetSerializer_default()
        {
            var factories = new List<IExportFactory<ISerializer, SerializerMetadata>>();
            factories.Add(this.GetSerializerFactory(typeof(JsonMediaType)));
            var injectableFactory = Substitute.For<IInjectableFactory>();
            var serializationService = new DefaultSerializationService(injectableFactory, factories);
            injectableFactory.Create<SerializationContext>(serializationService)
                .Returns(ci => new SerializationContext(Substitute.For<IServiceProvider>(), serializationService));

            ISerializationContext context = null;
            serializationService.Deserialize("123", ctx => context = ctx);

            Assert.AreSame(context.MediaType, typeof(JsonMediaType));
        }

        [Test]
        public void GetSerializer_proper_serializer_override()
        {
            var factories = new List<IExportFactory<ISerializer, SerializerMetadata>>();
            var oldSerializer = Substitute.For<ISerializer>();
            var newSerializer = Substitute.For<ISerializer>();
            factories.Add(this.GetSerializerFactory(typeof(JsonMediaType), oldSerializer, Priority.Normal));
            factories.Add(this.GetSerializerFactory(typeof(JsonMediaType), newSerializer, Priority.AboveNormal));
            var injectableFactory = Substitute.For<IInjectableFactory>();
            var serializationService = new DefaultSerializationService(injectableFactory, factories);
            injectableFactory.Create<SerializationContext>(serializationService)
                .Returns(ci => new SerializationContext(Substitute.For<IServiceProvider>(), serializationService));

            serializationService.Deserialize("123");
            oldSerializer.Received(0)
                .Deserialize(Arg.Any<string>(), Arg.Any<ISerializationContext>());
            newSerializer.Received(1)
                .Deserialize(Arg.Any<string>(), Arg.Any<ISerializationContext>());
        }

        [Test]
        public void Deserialize_object()
        {
            var serializer = Substitute.For<ISerializer>();
            var serializationService = this.CreateSerializationServiceForJson(serializer);
            serializer.Deserialize("123", Arg.Any<ISerializationContext>())
                .Returns(ci => "234");

            var result = serializationService.Deserialize("123");

            Assert.AreEqual("234", result);
        }

        [Test]
        public void Deserialize_object_sync()
        {
            var serializer = Substitute.For<ISerializer>();
            var serializationService = this.CreateSerializationServiceForJson(serializer);
            serializer.Deserialize("123", Arg.Any<ISerializationContext>())
                .Returns(ci => "234");

            var result = serializationService.Deserialize("123");

            Assert.AreEqual("234", result);
        }

        [Test]
        public void Deserialize_textreader()
        {
            var serializer = Substitute.For<ISerializer>();
            var serializationService = this.CreateSerializationServiceForJson(serializer);
            var reader = Substitute.For<TextReader>();
            serializer.Deserialize(reader, Arg.Any<ISerializationContext>())
                .Returns(ci => "234");

            var result = serializationService.Deserialize(reader);

            Assert.AreEqual("234", result);
        }

        [Test]
        public void Deserialize_textreader_sync()
        {
            var serializer = Substitute.For<ISerializer>();
            var serializationService = this.CreateSerializationServiceForJson(serializer);
            var reader = Substitute.For<TextReader>();
            serializer.Deserialize(reader, Arg.Any<ISerializationContext>())
                .Returns(ci => "234");

            var result = serializationService.Deserialize(reader);

            Assert.AreEqual("234", result);
        }

        [Test]
        public async Task DeserializeAsync_object()
        {
            var serializer = Substitute.For<ISerializer>();
            var serializationService = this.CreateSerializationServiceForJson(serializer);
            serializer.DeserializeAsync("123", Arg.Any<ISerializationContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult<object>("234"));

            var result = await serializationService.DeserializeAsync("123");

            Assert.AreEqual("234", result);
        }

        [Test]
        public async Task DeserializeAsync_textreader()
        {
            var serializer = Substitute.For<ISerializer>();
            var serializationService = this.CreateSerializationServiceForJson(serializer);
            var reader = Substitute.For<TextReader>();
            serializer.DeserializeAsync(reader, Arg.Any<ISerializationContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult<object>("234"));

            var result = await serializationService.DeserializeAsync(reader);

            Assert.AreEqual("234", result);
        }

#if NETCOREAPP3_1_OR_GREATER
        [Test]
        public void Serialize_object()
        {
            var serializer = Substitute.For<ISerializer>();
            var serializationService = this.CreateSerializationServiceForJson(serializer);
            serializer.Serialize("123", Arg.Any<ISerializationContext>())
                .Returns(ci => "234");

            var result = serializationService.Serialize("123");

            Assert.AreEqual("234", result);
        }
#else
        [Test]
        public void Serialize_object()
        {
            var serializer = Substitute.For<ISerializer>();
            var serializationService = this.CreateSerializationServiceForJson(serializer);
            serializer.SerializeAsync("123", Arg.Any<ISerializationContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult("234"));

            var result = serializationService.Serialize("123");

            Assert.AreEqual("234", result);
        }
#endif

#if NETCOREAPP3_1_OR_GREATER
        [Test]
        public void Serialize_object_sync()
        {
            var serializer = Substitute.For<ISerializer>();
            var serializationService = this.CreateSerializationServiceForJson(serializer);
            serializer.Serialize("123", Arg.Any<ISerializationContext>())
                .Returns(ci => "234");

            var result = serializationService.Serialize("123");

            Assert.AreEqual("234", result);
        }
#else
        [Test]
        public void Serialize_object_sync()
        {
            var serializer = Substitute.For<ISerializer, ISyncSerializer>();
            var serializationService = this.CreateSerializationServiceForJson(serializer);
            ((ISyncSerializer)serializer).Serialize("123", Arg.Any<ISerializationContext>())
                .Returns(ci => "234");

            var result = serializationService.Serialize("123");

            Assert.AreEqual("234", result);
        }
#endif

#if NETCOREAPP3_1_OR_GREATER
        [Test]
        public void Serialize_textwriter()
        {
            var serializer = Substitute.For<ISerializer>();
            var serializationService = this.CreateSerializationServiceForJson(serializer);
            var writer = new StringWriter();
            serializer.When(s => s.Serialize("123", writer, Arg.Any<ISerializationContext>()))
                .Do(ci => writer.Write(234));

            serializationService.Serialize("123", writer);

            Assert.AreEqual("234", writer.GetStringBuilder().ToString());
        }
#else
        [Test]
        public void Serialize_textwriter()
        {
            var serializer = Substitute.For<ISerializer>();
            var serializationService = this.CreateSerializationServiceForJson(serializer);
            var writer = new StringWriter();
            serializer.SerializeAsync("123", writer, Arg.Any<ISerializationContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.CompletedTask)
                .AndDoes(ci => writer.Write(234));

            serializationService.Serialize("123", writer);

            Assert.AreEqual("234", writer.GetStringBuilder().ToString());
        }
#endif

#if NETCOREAPP3_1_OR_GREATER
        [Test]
        public void Serialize_textwriter_sync()
        {
            var serializer = Substitute.For<ISerializer>();
            var serializationService = this.CreateSerializationServiceForJson(serializer);
            var writer = new StringWriter();
            serializer
                .When(s => s.Serialize("123", writer, Arg.Any<ISerializationContext>()))
                .Do(ci => writer.Write("234"));

            serializationService.Serialize("123", writer);

            Assert.AreEqual("234", writer.GetStringBuilder().ToString());
        }
#else
        [Test]
        public void Serialize_textwriter_sync()
        {
            var serializer = Substitute.For<ISerializer, ISyncSerializer>();
            var serializationService = this.CreateSerializationServiceForJson(serializer);
            var writer = new StringWriter();
            ((ISyncSerializer)serializer)
                .When(s => s.Serialize("123", writer, Arg.Any<ISerializationContext>()))
                .Do(ci => writer.Write("234"));

            serializationService.Serialize("123", writer);

            Assert.AreEqual("234", writer.GetStringBuilder().ToString());
        }
#endif

        [Test]
        public async Task SerializeAsync_object()
        {
            var serializer = Substitute.For<ISerializer>();
            var serializationService = this.CreateSerializationServiceForJson(serializer);
            serializer.SerializeAsync("123", Arg.Any<ISerializationContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult("234"));

            var result = await serializationService.SerializeAsync("123");

            Assert.AreEqual("234", result);
        }

        [Test]
        public async Task SerializeAsync_textwriter()
        {
            var serializer = Substitute.For<ISerializer>();
            var serializationService = this.CreateSerializationServiceForJson(serializer);
            var writer = new StringWriter();
            serializer.SerializeAsync("123", writer, Arg.Any<ISerializationContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.CompletedTask)
                .AndDoes(ci => writer.Write(234));

            await serializationService.SerializeAsync("123", writer);

            Assert.AreEqual("234", writer.GetStringBuilder().ToString());
        }

        private DefaultSerializationService CreateSerializationServiceForJson(ISerializer? serializer = null)
        {
            var factories = new List<IExportFactory<ISerializer, SerializerMetadata>>();
            factories.Add(this.GetSerializerFactory(typeof(JsonMediaType), serializer));
            var injectableFactory = Substitute.For<IInjectableFactory>();
            var serializationService = new DefaultSerializationService(injectableFactory, factories);
            injectableFactory.Create<SerializationContext>(serializationService)
                .Returns(ci => new SerializationContext(Substitute.For<IServiceProvider>(), serializationService));

            return serializationService;
        }

        private IExportFactory<ISerializer, SerializerMetadata> GetSerializerFactory(
            Type mediaType,
            ISerializer? serializer = null,
            Priority overridePriority = Priority.Normal)
        {
            var metadata = new SerializerMetadata(mediaType, overridePriority: overridePriority);
            serializer ??= Substitute.For<ISerializer>();
            var factory = new ExportFactory<ISerializer, SerializerMetadata>(() => serializer, metadata);
            return factory;
        }
    }
}