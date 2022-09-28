// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStreamReaderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data stream reader test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Tests.DataStreams
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Net.Mime;
    using Kephas.Serialization;
    using Kephas.Testing;
    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DataStreamReaderTest : TestBase
    {
        [Test]
        public async Task ReadAsync_entity_check()
        {
            var entity = "123";

            var mediaTypeProvider = Substitute.For<IMediaTypeProvider>();
            mediaTypeProvider.GetMediaType(Arg.Any<string>(), Arg.Any<bool>()).Returns(typeof(JsonMediaType));

            var serializer = Substitute.For<ISerializer>();
            serializer.DeserializeAsync(Arg.Any<TextReader>(), Arg.Any<ISerializationContext>(), Arg.Any<CancellationToken>())
                .Returns(entity);

            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(serializer);

            var reader = new DataStreamReader(serializationService, mediaTypeProvider);
            using var dataStream = new DataStream(new MemoryStream(new byte[] { 0, 1, 2 }), "test", ownsStream: true);
            var result = await reader.ReadAsync(dataStream, Substitute.For<IDataIOContext>());
            Assert.AreSame(entity, result);
        }

        [Test]
        public async Task ReadAsync_provided_RootObjectType()
        {
            var entity = "123";
            ISerializationContext serializationContext = null;

            var mediaTypeProvider = Substitute.For<IMediaTypeProvider>();
            mediaTypeProvider.GetMediaType(Arg.Any<string>(), Arg.Any<bool>()).Returns(typeof(JsonMediaType));

            var serializer = Substitute.For<ISerializer>();
            serializer.DeserializeAsync(Arg.Any<TextReader>(), Arg.Any<ISerializationContext>(), Arg.Any<CancellationToken>())
                .Returns(entity)
                .AndDoes(ci => serializationContext = ci.Arg<ISerializationContext>());

            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(serializer);

            var reader = new DataStreamReader(serializationService, mediaTypeProvider);
            using var dataStream = new DataStream(new MemoryStream(new byte[] { 0, 1, 2 }), "test", ownsStream: true);
            await reader.ReadAsync(dataStream, new DataIOContext(Substitute.For<IServiceProvider>()).RootObjectType(typeof(bool)));
            Assert.IsNotNull(serializationContext);
            Assert.AreEqual(typeof(bool), serializationContext.RootObjectType);
        }

        [Test]
        public async Task ReadAsync_default_RootObjectType()
        {
            var entity = "123";
            ISerializationContext serializationContext = null;

            var mediaTypeProvider = Substitute.For<IMediaTypeProvider>();
            mediaTypeProvider.GetMediaType(Arg.Any<string>(), Arg.Any<bool>()).Returns(typeof(JsonMediaType));

            var serializer = Substitute.For<ISerializer>();
            serializer.DeserializeAsync(Arg.Any<TextReader>(), Arg.Any<ISerializationContext>(), Arg.Any<CancellationToken>())
                .Returns(entity)
                .AndDoes(ci => serializationContext = ci.Arg<ISerializationContext>());

            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(serializer);

            var reader = new DataStreamReader(serializationService, mediaTypeProvider);
            using var dataStream = new DataStream(new MemoryStream(new byte[] { 0, 1, 2 }), "test", ownsStream: true);
            await reader.ReadAsync(dataStream, Substitute.For<IDataIOContext>());
            Assert.IsNotNull(serializationContext);
            Assert.AreEqual(typeof(List<object>), serializationContext.RootObjectType);
        }
    }
}