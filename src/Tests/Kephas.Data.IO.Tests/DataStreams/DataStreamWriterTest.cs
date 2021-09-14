// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStreamWriterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data stream writer test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Tests.DataStreams
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Net.Mime;
    using Kephas.Serialization;
    using Kephas.Testing;
    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DataStreamWriterTest : TestBase
    {
        [Test]
        public async Task WriteAsync_entity_check()
        {
            var serializedEntity = "123";

            var mediaTypeProvider = Substitute.For<IMediaTypeProvider>();
            mediaTypeProvider.GetMediaType(Arg.Any<string>(), Arg.Any<bool>()).Returns(typeof(JsonMediaType));

            var serializer = Substitute.For<ISerializer>();
            serializer.SerializeAsync(Arg.Any<object>(), Arg.Any<ISerializationContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult(serializedEntity));

            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(serializer);

            var writer = new DataStreamWriter(serializationService, mediaTypeProvider);
            var memStream = new MemoryStream();
            using (var dataStream = new DataStream(memStream, "test", ownsStream: true))
            {
                await writer.WriteAsync(new[] { "abcd" }, dataStream);
                var str = Encoding.UTF8.GetString(memStream.ToArray()).Substring(1); // cut the first unicode char
                Assert.AreEqual(serializedEntity, str);
            }
        }

        [Test]
        public async Task WriteAsync_provided_RootObjectType()
        {
            var serializedEntity = "123";
            ISerializationContext serializationContext = null;

            var mediaTypeProvider = Substitute.For<IMediaTypeProvider>();
            mediaTypeProvider.GetMediaType(Arg.Any<string>(), Arg.Any<bool>()).Returns(typeof(JsonMediaType));

            var serializer = Substitute.For<ISerializer>();
            serializer.SerializeAsync(Arg.Any<object>(), Arg.Any<ISerializationContext>(), Arg.Any<CancellationToken>())
                .Returns(ci =>
                {
                    serializationContext = ci.Arg<ISerializationContext>();
                    return Task.FromResult("abc");
                });

            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(serializer);

            var writer = new DataStreamWriter(serializationService, mediaTypeProvider);
            var memStream = new MemoryStream();
            using (var dataStream = new DataStream(memStream, "test", ownsStream: true))
            {
                await writer.WriteAsync(new[] { "abcd" }, dataStream, new DataIOContext(Substitute.For<IInjector>()).RootObjectType(typeof(bool)));
                Assert.IsNotNull(serializationContext);
                Assert.AreEqual(typeof(bool), serializationContext.RootObjectType);
            }
        }

        [Test]
        public async Task WriteAsync_default_RootObjectType()
        {
            var serializedEntity = "123";
            ISerializationContext serializationContext = null;

            var mediaTypeProvider = Substitute.For<IMediaTypeProvider>();
            mediaTypeProvider.GetMediaType(Arg.Any<string>(), Arg.Any<bool>()).Returns(typeof(JsonMediaType));

            var serializer = Substitute.For<ISerializer>();
            serializer.SerializeAsync(Arg.Any<object>(), Arg.Any<ISerializationContext>(), Arg.Any<CancellationToken>())
                .Returns(ci =>
                {
                    serializationContext = ci.Arg<ISerializationContext>();
                    return Task.FromResult("abc");
                });

            var serializationService = this.CreateSerializationServiceMock<JsonMediaType>(serializer);

            var writer = new DataStreamWriter(serializationService, mediaTypeProvider);
            var memStream = new MemoryStream();
            using (var dataStream = new DataStream(memStream, "test", ownsStream: true))
            {
                await writer.WriteAsync("abcd", dataStream);
                Assert.IsNotNull(serializationContext);
                Assert.AreEqual(typeof(string), serializationContext.RootObjectType);
            }
        }
    }
}